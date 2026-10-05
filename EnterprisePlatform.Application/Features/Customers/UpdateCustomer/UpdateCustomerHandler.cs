using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Application.Exceptions;
using EnterprisePlatform.Application.Features.Customers.UpdateCustomer;
using MediatR;

public sealed class UpdateCustomerHandler
    : IRequestHandler<UpdateCustomerCommand, long>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<long> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
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

        _customerRepository.SetExpectedVersion(customer, request.ExpectedVersion);

        customer.UpdateDetails(request.FirstName, request.LastName, request.Email, request.PhoneNumber, request.DateOfBirth);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Version;
    }
}