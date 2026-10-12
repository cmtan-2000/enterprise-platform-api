
using System.Text.Json;
using System.Threading.Channels;
using EnterprisePlatform.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace EnterprisePlatform.Infrastructure.Caching;

public sealed class RedisCacheInvalidationSubscriber : BackgroundService
{
    private const string ChannelName =
        "EnterprisePlatform:cache-invalidation";

    private static readonly TimeSpan InitialRetryDelay =
        TimeSpan.FromSeconds(1);

    private static readonly TimeSpan MaxRetryDelay =
        TimeSpan.FromSeconds(30);

    private readonly IConnectionMultiplexer _redis;
    private readonly HybridCache _cache;
    private readonly ILogger<RedisCacheInvalidationSubscriber> _logger;

    private readonly Channel<string> _messages =
        Channel.CreateUnbounded<string>();

    public RedisCacheInvalidationSubscriber(
        IConnectionMultiplexer redis,
        HybridCache cache,
        ILogger<RedisCacheInvalidationSubscriber> logger)
    {
        _redis = redis;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var subscriber = _redis.GetSubscriber();

        var channel = RedisChannel.Literal(ChannelName);

        Action<RedisChannel, RedisValue> messageHandler =
            (_, message) =>
            {
                if (!message.IsNullOrEmpty)
                {
                    _messages.Writer.TryWrite(message.ToString());
                }
            };

        var subscribed = false;
        var retryDelay = InitialRetryDelay;

        try
        {
            // Retry subscription when Redis is temporarily unavailable.
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await subscriber.SubscribeAsync(
                        channel,
                        messageHandler);

                    subscribed = true;

                    _logger.LogInformation(
                        "Subscribed to Redis cache invalidation channel");

                    break;
                }
                catch (RedisException ex)
                    when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning(
                        ex,
                        "Redis subscription failed. Retrying in {RetryDelaySeconds} seconds",
                        retryDelay.TotalSeconds);

                    await Task.Delay(retryDelay, stoppingToken);

                    retryDelay = TimeSpan.FromSeconds(
                        Math.Min(
                            retryDelay.TotalSeconds * 2,
                            MaxRetryDelay.TotalSeconds));
                }
            }

            if (!subscribed)
            {
                return;
            }

            // Process invalidation messages after subscription succeeds.
            await foreach (
                var payload in _messages.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    var message =
                        JsonSerializer.Deserialize
                            <CustomerCacheInvalidationMessage>(payload);

                    if (message is null)
                    {
                        _logger.LogWarning(
                            "Received invalid cache invalidation message");

                        continue;
                    }

                    await _cache.RemoveAsync(
                        $"customer:{message.CustomerId}",
                        stoppingToken);

                    _logger.LogInformation(
                        "Invalidated customer cache for {CustomerId}",
                        message.CustomerId);
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Failed to deserialize cache invalidation message");
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                    when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogError(
                        ex,
                        "Failed to process cache invalidation message");
                }
            }
        }
        finally
        {
            _messages.Writer.TryComplete();

            if (subscribed)
            {
                try
                {
                    await subscriber.UnsubscribeAsync(
                        channel,
                        messageHandler);
                }
                catch (RedisException ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Failed to unsubscribe from Redis during shutdown");
                }
            }
        }
    }
}
