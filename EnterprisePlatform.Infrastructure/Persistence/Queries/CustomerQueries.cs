using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EnterprisePlatform.Application.Abstractions.Pagination;
using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Application.Features.Customers.GetCustomers;
using EnterprisePlatform.Application.Features.Customers.GetCustomerSummary;
using EnterprisePlatform.Application.Observability;
using EnterprisePlatform.Domain.Entities;
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
            IQueryable<Customer> query;

            using (var activity = ApplicationActivitySource.Source.StartActivity("Customers.PrepareQuery"))
            {
                IQueryable<Customer> customers;

                var sw1 = System.Diagnostics.Stopwatch.StartNew();

                customers = _dbContext.Customers;

                sw1.Stop();

                Console.WriteLine(
                    $"DbSet access: {sw1.Elapsed.TotalMilliseconds:F3} ms");

                var sw2 = System.Diagnostics.Stopwatch.StartNew();

                query = customers.AsNoTracking();

                sw2.Stop();

                Console.WriteLine(
                    $"AsNoTracking: {sw2.Elapsed.TotalMilliseconds:F3} ms");
            }

            CustomerCursor? cursor = null;

            using (ApplicationActivitySource.Source.StartActivity("Customers.DecodeCursor"))
            {
                if (!string.IsNullOrWhiteSpace(request.Cursor))
                {
                    cursor = _cursorSerializer
                        .Decode<CustomerCursor>(request.Cursor);
                }
            }

            using (ApplicationActivitySource.Source.StartActivity("Customers.ApplyCursorFilter"))
            {
                if (cursor is not null)
                {
                    query = query.Where(x =>
                        x.CreatedAt < cursor.CreatedAt ||
                        (x.CreatedAt == cursor.CreatedAt &&
                         x.Id.CompareTo(cursor.Id) < 0));
                }
            }

            List<CustomerListItem> items;

            using (var activity = ApplicationActivitySource.Source
                .StartActivity("Customers.ExecuteQuery"))
            {
                var stopwatch = Stopwatch.StartNew();

                items = await query
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

                stopwatch.Stop();

                activity?.SetTag(
                    "app.query.duration_ms",
                    stopwatch.Elapsed.TotalMilliseconds);

                activity?.SetTag(
                    "app.query.result_count",
                    items.Count);
            }

            using (var activity = ApplicationActivitySource.Source.StartActivity("Customers.BuildResult"))
            {
                var hasNextPage = items.Count > request.PageSize;

                var pageItems = hasNextPage
                    ? items.Take(request.PageSize).ToList()
                    : items;

                var nextCursor = hasNextPage
                    ? _cursorSerializer.Encode(
                        new CustomerCursor(
                            pageItems[^1].CreatedAt,
                            pageItems[^1].Id))
                    : null;

                return new GetCustomersResult(
                    pageItems,
                    request.PageSize,
                    nextCursor);
            }
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