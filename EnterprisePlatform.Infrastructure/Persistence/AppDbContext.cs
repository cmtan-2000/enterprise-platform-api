using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Application.Exceptions;
using EnterprisePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EnterprisePlatform.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    IQueryable<Customer> IApplicationDbContext.Customers => Customers;

    public async Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken)
    {
        await Customers.AddAsync(customer, cancellationToken);
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    {
        await using var transaction = await Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await operation(cancellationToken);

            await SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch (DbUpdateException ex) when (IsIdempotencyConflict(ex))
        {
            await transaction.RollbackAsync(cancellationToken);

            throw new IdempotencyConflictException(
                "The Idempotency-Key has already been processed.",
                ex);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    private static bool IsIdempotencyConflict(
    DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException
            && postgresException.SqlState == PostgresErrorCodes.UniqueViolation
            && postgresException.ConstraintName == "PK_idempotency_records";
    }
}