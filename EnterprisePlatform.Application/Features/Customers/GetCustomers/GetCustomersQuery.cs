using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace EnterprisePlatform.Application.Features.Customers.GetCustomers
{
    public sealed record GetCustomersQuery(
    int PageSize = 20,
    string? Search = null,
    string? Cursor = null
    ) : IRequest<GetCustomersResult>;
}