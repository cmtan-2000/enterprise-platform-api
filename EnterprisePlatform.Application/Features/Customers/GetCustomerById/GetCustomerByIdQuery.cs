using MediatR;

namespace EnterprisePlatform.Application.Features.Customers.GetCustomerById;

public sealed record GetCustomerByIdQuery(
    Guid CustomerId
) : IRequest<GetCustomerByIdResult>;