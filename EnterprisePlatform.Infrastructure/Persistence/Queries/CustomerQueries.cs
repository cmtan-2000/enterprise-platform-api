using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Application.Features.Customers.GetCustomers;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePlatform.Infrastructure.Persistence.Queries
{
    public class CustomerQueries : ICustomerQueries
    {
        private readonly AppDbContext _dbContext;

        public CustomerQueries(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetCustomersResult> GetCustomersAsync(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Customers
                .AsNoTracking();

            // Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(x =>
                    EF.Functions.ILike(x.FirstName, $"%{search}%") ||
                    EF.Functions.ILike(x.LastName, $"%{search}%") ||
                    EF.Functions.ILike(x.Email, $"%{search}%"));
            }

            // Count
            var totalCount = await query.CountAsync(
                cancellationToken);

            // Sort
            query = request.SortBy?.ToLowerInvariant() switch
            {
                "firstname" => request.SortDirection == "desc"
                    ? query
                        .OrderByDescending(x => x.FirstName)
                        .ThenBy(x => x.Id)
                    : query
                        .OrderBy(x => x.FirstName)
                        .ThenBy(x => x.Id),

                "lastname" => request.SortDirection == "desc"
                    ? query
                        .OrderByDescending(x => x.LastName)
                        .ThenBy(x => x.Id)
                    : query
                        .OrderBy(x => x.LastName)
                        .ThenBy(x => x.Id),

                "email" => request.SortDirection == "desc"
                    ? query
                        .OrderByDescending(x => x.Email)
                        .ThenBy(x => x.Id)
                    : query
                        .OrderBy(x => x.Email)
                        .ThenBy(x => x.Id),

                "dateofbirth" => request.SortDirection == "desc"
                    ? query
                        .OrderByDescending(x => x.DateOfBirth)
                        .ThenBy(x => x.Id)
                    : query
                        .OrderBy(x => x.DateOfBirth)
                        .ThenBy(x => x.Id),

                _ => query
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.Id)
            };

            // Pagination
            var skip = (request.Page - 1) * request.PageSize;

            var items = await query
                .Skip(skip)
                .Take(request.PageSize)
                .Select(x => new CustomerListItem(
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Email))
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)request.PageSize);

            return new GetCustomersResult(
                items,
                request.Page,
                request.PageSize,
                totalCount,
                totalPages);
        }

    }
}