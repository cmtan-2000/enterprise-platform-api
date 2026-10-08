using EnterprisePlatform.Domain.Entities;

public interface IIdempotencyRepository
{
    Task<IdempotencyRecord?> GetByKeyAsync(
        string key,
        CancellationToken cancellationToken);

    Task AddAsync(
        IdempotencyRecord record,
        CancellationToken cancellationToken);
}