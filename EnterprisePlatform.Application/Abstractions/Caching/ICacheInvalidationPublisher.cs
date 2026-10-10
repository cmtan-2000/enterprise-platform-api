using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterprisePlatform.Application.Abstractions.Caching
{
    public interface ICacheInvalidationPublisher
    {
        Task PublishCustomerInvalidationAsync(Guid customerId, CancellationToken cancellationToken);
    }
}