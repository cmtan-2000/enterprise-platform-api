using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePlatform.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
	private readonly AppDbContext _dbContext;

	public CustomerRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public IQueryable<Customer> Query()
	{
		return _dbContext.Customers;
	}

	public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _dbContext.Customers
			.FirstOrDefaultAsync(
				x => x.Id == id,
				cancellationToken);
	}

	public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
	{
		await _dbContext.Customers.AddAsync(
			customer,
			cancellationToken);
	}

	public void SetExpectedVersion(Customer customer, long expectedVersion)
	{
		_dbContext.Entry(customer)
			.Property(x => x.Version)
			.OriginalValue = expectedVersion;
	}
}