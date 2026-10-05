using EnterprisePlatform.Domain.Entities;

namespace EnterprisePlatform.Application.Abstractions.Persistence;

public interface ICustomerRepository
{
	Task<Customer?> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken);

	Task AddAsync(
		Customer customer,
		CancellationToken cancellationToken);

	void SetExpectedVersion(Customer customer, long expectedVersion);
}