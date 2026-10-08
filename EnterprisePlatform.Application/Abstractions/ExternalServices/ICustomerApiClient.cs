using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterprisePlatform.Application.Abstractions.ExternalServices;

public interface ICustomerApiClient
{
    Task<CustomerProfileDto?> GetProfileAsync(
        Guid customerId,
        CancellationToken cancellationToken);
}