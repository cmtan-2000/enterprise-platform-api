using EnterprisePlatform.Domain.Entities;

namespace EnterprisePlatform.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    IQueryable<Customer> Customers { get; }

    Task AddCustomerAsync(
        Customer customer,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}