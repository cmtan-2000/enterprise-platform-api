using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterprisePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePlatform.Infrastructure.Persistence.Repositories
{
    public sealed class IdempotencyRepository
    : IIdempotencyRepository
    {
        private readonly AppDbContext _dbContext;

        public IdempotencyRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IdempotencyRecord?> GetByKeyAsync(
            string key,
            CancellationToken cancellationToken)
        {
            return await _dbContext.IdempotencyRecords
                .FirstOrDefaultAsync(
                    x => x.Key == key,
                    cancellationToken);
        }

        public async Task AddAsync(
            IdempotencyRecord record,
            CancellationToken cancellationToken)
        {
            await _dbContext.IdempotencyRecords.AddAsync(
                record,
                cancellationToken);
        }
    }
}