using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterprisePlatform.Application.Features.Customers.GetCustomers;

namespace EnterprisePlatform.Application.Abstractions.Persistence;

public interface ICustomerQueries
{
    Task<GetCustomersResult> GetCustomersAsync(GetCustomersQuery request, CancellationToken cancellationToken);

    Task TestNPlusOneAsync(CancellationToken cancellationToken);
}