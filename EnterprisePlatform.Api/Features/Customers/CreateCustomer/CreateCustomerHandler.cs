using MediatR;
using EnterprisePlatform.Domain.Entities;

namespace EnterprisePlatform.Application.Features.Customers.CreateCustomer;

public sealed class CreateCustomerHandler
    : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
{
    public Task<CreateCustomerResult> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = new Customer(
            command.FirstName,
            command.LastName,
            command.Email,
            command.PhoneNumber,
            command.DateOfBirth);

        var result = new CreateCustomerResult(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email);

        return Task.FromResult(result);
    }
}