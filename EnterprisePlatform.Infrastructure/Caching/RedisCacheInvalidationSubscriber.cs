
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

        await subscriber.SubscribeAsync(
            RedisChannel.Literal(ChannelName),
            (_, message) =>
            {
                if (!message.IsNullOrEmpty)
                {
                    _messages.Writer.TryWrite(message.ToString());
                }
            });

        _logger.LogInformation(
            "Subscribed to Redis cache invalidation channel");

        try
        {
            await foreach (
                var payload in _messages.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    var message =
                        JsonSerializer.Deserialize<CustomerCacheInvalidationMessage>(
                            payload);

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
                catch (Exception ex) when (
                    !stoppingToken.IsCancellationRequested)
                {
                    _logger.LogError(
                        ex,
                        "Failed to process cache invalidation message");
                }
            }
        }
        finally
        {
            await subscriber.UnsubscribeAsync(
                RedisChannel.Literal(ChannelName));
        }
    }
}
