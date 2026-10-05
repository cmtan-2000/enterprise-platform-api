using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace EnterprisePlatform.Application.Features.Customers.UpdateCustomer
{
    public sealed record UpdateCustomerCommand(
    Guid CustomerId,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    DateTime DateOfBirth,
    long ExpectedVersion) : IRequest<long>;
}