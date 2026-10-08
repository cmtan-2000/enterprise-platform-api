using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePlatform.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Address> Addresses => Set<Address>();

    IQueryable<Customer> IApplicationDbContext.Customers => Customers;

    public async Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken)
    {
        await Customers.AddAsync(customer, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}