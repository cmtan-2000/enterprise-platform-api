using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterprisePlatform.Application.Abstractions.Persistence
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken);
    }
}