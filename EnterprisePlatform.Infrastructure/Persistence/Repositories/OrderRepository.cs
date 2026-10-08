using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Domain.Entities;

namespace EnterprisePlatform.Infrastructure.Persistence.Repositories
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _dbContext;

        public OrderRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Order order, CancellationToken cancellationToken)
        {
            await _dbContext.Orders.AddAsync(
                order,
                cancellationToken);
        }
    }
}