using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterprisePlatform.Application.Abstractions.Pagination;
using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Application.Features.Customers.GetCustomers;
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
                query = query.Where(x => string.Compare(x.Email, cursor.Email) > 0);
            }

            var items = await query
                .OrderBy(x => x.Email)
                .Take(request.PageSize + 1)
                .Select(x => new CustomerListItem(
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Email))
                .ToListAsync(cancellationToken);

            var hasNextPage = items.Count > request.PageSize;

            var pageItems = hasNextPage
                ? items.Take(request.PageSize).ToList()
                : items;

            var nextCursor = hasNextPage
                ? _cursorSerializer.Encode(
                    new CustomerCursor(pageItems[^1].Email))
                : null;

            return new GetCustomersResult(
                pageItems,
                request.PageSize,
                nextCursor
            );
        }

    }
}