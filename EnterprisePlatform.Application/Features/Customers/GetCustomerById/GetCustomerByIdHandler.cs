using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Application.Exceptions;
using MediatR;

namespace EnterprisePlatform.Application.Features.Customers.GetCustomerById;

public sealed class GetCustomerByIdHandler
    : IRequestHandler<GetCustomerByIdQuery, GetCustomerByIdResult>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdHandler(
        ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<GetCustomerByIdResult> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            request.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException(
                "Customer",
                request.CustomerId);
        }

        return new GetCustomerByIdResult(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.PhoneNumber,
            customer.DateOfBirth,
            customer.Version);
    }
}