using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterprisePlatform.Application.Abstractions.Pagination;
using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Application.Features.Customers.GetCustomers;
using EnterprisePlatform.Application.Features.Customers.GetCustomerSummary;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePlatform.Infrastructure.Persistence.Queries
{
    public class CustomerQueries : ICustomerQueries
    {
        private readonly AppDbContext _dbContext;
        private readonly ICursorSerializer _cursorSerializer;

        public CustomerQueries(AppDbContext dbContext, ICursorSerializer cursorSerializer)
        {
            _dbContext = dbContext;
            _cursorSerializer = cursorSerializer;
        }

        public async Task<GetCustomersResult> GetCustomersAsync(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Customers
                .AsNoTracking();

            // Cursor
            CustomerCursor? cursor = null;
            if (!string.IsNullOrWhiteSpace(request.Cursor))
            {
                cursor = _cursorSerializer.Decode<CustomerCursor>(request.Cursor);
            }

            if (cursor is not null)
            {
                query = query.Where(x =>
                    x.CreatedAt < cursor.CreatedAt || (x.CreatedAt == cursor.CreatedAt
                    && x.Id.CompareTo(cursor.Id) < 0));
            }

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Take(request.PageSize + 1)
                .Select(x => new CustomerListItem(
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Email,
                    x.CreatedAt))
                .ToListAsync(cancellationToken);

            var hasNextPage = items.Count > request.PageSize;

            var pageItems = hasNextPage
                ? items.Take(request.PageSize).ToList()
                : items;

            var nextCursor = hasNextPage
                ? _cursorSerializer.Encode(
                    new CustomerCursor(pageItems[^1].CreatedAt, pageItems[^1].Id))
                : null;

            return new GetCustomersResult(
                pageItems,
                request.PageSize,
                nextCursor
            );
        }
        public async Task TestNPlusOneAsync(CancellationToken cancellationToken)
        {
            var customers = await _dbContext.Customers
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Take(10)
                .Select(x => new CustomerSummaryDto(
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Orders.Count,
                    x.Addresses.Count))
                .ToListAsync(cancellationToken);

        }
    }
}