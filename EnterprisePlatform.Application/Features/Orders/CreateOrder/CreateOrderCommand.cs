using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace EnterprisePlatform.Application.Features.Orders.CreateOrder
{
    public sealed record CreateOrderCommand(
        Guid CustomerId,
        decimal Amount,
        string IdempotencyKey
    ) : IRequest<Guid>;
}