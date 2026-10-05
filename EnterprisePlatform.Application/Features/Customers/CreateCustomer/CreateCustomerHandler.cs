using MediatR;
using EnterprisePlatform.Domain.Entities;
using EnterprisePlatform.Application.Abstractions.Persistence;

namespace EnterprisePlatform.Application.Features.Customers.CreateCustomer;

public sealed class CreateCustomerHandler
    : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCustomerResult> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {

        var customer = new Customer(
            command.FirstName,
            command.LastName,
            command.Email,
            command.PhoneNumber,
            command.DateOfBirth);

        await _customerRepository.AddAsync(customer, cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateCustomerResult(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email
        );
    }
}