namespace EnterprisePlatform.Application.Abstractions.Caching;

public sealed record CustomerCacheInvalidationMessage(
    Guid CustomerId);