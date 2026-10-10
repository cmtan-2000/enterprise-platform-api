using System.Text.Json;
using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;

namespace EnterprisePlatform.Application.Features.Customers.GetCustomerById;

public sealed class GetCustomerByIdHandler
    : IRequestHandler<GetCustomerByIdQuery, GetCustomerByIdResult>
{
    private readonly ICustomerRepository _customerRepository;
    // private readonly IMemoryCache _cache;
    // private readonly IDistributedCache _cache;
    private readonly HybridCache _cache;

    public GetCustomerByIdHandler(
        ICustomerRepository customerRepository,
        // IMemoryCache cache,
        // IDistributedCache cache,
        HybridCache cache)
    {
        _customerRepository = customerRepository;
        _cache = cache;
        Console.WriteLine($"Distributed cache implementation: {_cache.GetType().FullName}");
    }

    public async Task<GetCustomerByIdResult> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"customer:{request.CustomerId}";
        Console.WriteLine(cacheKey);

        // Cache Hit

        #region IMemoryCache
        // if (_cache.TryGetValue(cacheKey, out GetCustomerByIdResult? cachedCustomer))
        // {
        //     Console.WriteLine("CACHE HIT");
        //     return cachedCustomer;
        // }
        #endregion

        #region Redis + IDistributedCache
        // var cachedJson = await _cache.GetStringAsync(cacheKey, cancellationToken);
        // if (cachedJson is not null)
        // {
        //     var cachedResult = JsonSerializer.Deserialize<GetCustomerByIdResult>(cachedJson);

        //     if (cachedResult is not null)
        //     {
        //         Console.WriteLine("REDIS CACHE HIT");
        //         return cachedResult;
        //     }
        // }

        // await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result),
        //     new DistributedCacheEntryOptions
        //     {
        //         AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        //     },
        //     cancellationToken);

        // var cachedTypeCheck = await _cache.GetStringAsync(
        //     cacheKey,
        //     cancellationToken);
        #endregion

        #region HybridCache
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async token =>
            {
                Console.WriteLine("HYBRID CACHE MISS");

                var customer = await _customerRepository.GetByIdAsync(
                    request.CustomerId,
                    token);

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
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5),
                LocalCacheExpiration = TimeSpan.FromMinutes(1)
            },
            cancellationToken: cancellationToken);
        #endregion


        Console.WriteLine("CACHE MISS");

        var customer = await _customerRepository.GetByIdAsync(
            request.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException(
                "Customer",
                request.CustomerId);
        }

        var result = new GetCustomerByIdResult(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.PhoneNumber,
            customer.DateOfBirth,
            customer.Version);

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