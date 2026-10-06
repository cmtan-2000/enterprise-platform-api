using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace EnterprisePlatform.Application.Features.Customers.GetCustomers
{
    public sealed record GetCustomersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? SortBy = null,
    string? SortDirection = null
    ) : IRequest<GetCustomersResult>;
}