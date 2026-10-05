using MediatR;
using EnterprisePlatform.Domain.Entities;
using EnterprisePlatform.Application.Abstractions.Persistence;

namespace EnterprisePlatform.Application.Features.Customers.CreateCustomer;

public sealed class CreateCustomerHandler
    : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
{
    private readonly IApplicationDbContext _dbContext;
    public CreateCustomerHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateCustomerResult> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {

        var customer = new Customer(
            command.FirstName,
            command.LastName,
            command.Email,
            command.PhoneNumber,
            command.DateOfBirth);

        await _dbContext.AddCustomerAsync(
            customer,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return new CreateCustomerResult(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email
        );
    }
}