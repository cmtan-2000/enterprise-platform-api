
using System.Text.Json;
using EnterprisePlatform.Application.Abstractions.Caching;
using StackExchange.Redis;

namespace EnterprisePlatform.Infrastructure.Caching;

public sealed class RedisCacheInvalidationPublisher
    : ICacheInvalidationPublisher
{
    private const string ChannelName =
        "EnterprisePlatform:cache-invalidation";

    private readonly IConnectionMultiplexer _redis;

    public RedisCacheInvalidationPublisher(
        IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task PublishCustomerInvalidationAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var message = new CustomerCacheInvalidationMessage(customerId);
        var payload = JsonSerializer.Serialize(message);

        await _redis.GetSubscriber().PublishAsync(
            RedisChannel.Literal(ChannelName),
            payload);
    }
}
