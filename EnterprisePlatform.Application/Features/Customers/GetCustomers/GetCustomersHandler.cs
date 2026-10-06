using EnterprisePlatform.Application.Abstractions.Persistence;
using MediatR;

namespace EnterprisePlatform.Application.Features.Customers.GetCustomers;

public sealed class GetCustomersHandler : IRequestHandler<GetCustomersQuery, GetCustomersResult>
{
    private readonly ICustomerQueries _customerQueries;

    public GetCustomersHandler(ICustomerQueries customerQueries)
    {
        _customerQueries = customerQueries;
    }

    public async Task<GetCustomersResult> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        return await _customerQueries.GetCustomersAsync(
        request,
        cancellationToken);
    }
}