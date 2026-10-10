using EnterprisePlatform.Application.Abstractions.Caching;
using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Application.Exceptions;
using EnterprisePlatform.Application.Features.Customers.UpdateCustomer;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;

public sealed class UpdateCustomerHandler
    : IRequestHandler<UpdateCustomerCommand, long>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    // private readonly IMemoryCache _cache;
    // private readonly IDistributedCache _cache;
    private readonly HybridCache _cache;
    private readonly ICacheInvalidationPublisher _cacheInvalidationPublisher;

    public UpdateCustomerHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork,
        // IMemoryCache cache
        HybridCache cache,
        ICacheInvalidationPublisher cacheInvalidationPublisher)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
        _cacheInvalidationPublisher = cacheInvalidationPublisher;
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

        // _cache.Remove($"customer:{request.CustomerId}");
        await _cache.RemoveAsync($"customer:{request.CustomerId}", cancellationToken);

        await _cacheInvalidationPublisher
            .PublishCustomerInvalidationAsync(request.CustomerId, cancellationToken);

        return customer.Version;
    }
}