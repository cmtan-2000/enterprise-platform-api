using MediatR;

namespace EnterprisePlatform.Application.Features.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth)
    : IRequest<CreateCustomerResult>;