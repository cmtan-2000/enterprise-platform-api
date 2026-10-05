namespace EnterprisePlatform.Application.Features.Customers.GetCustomerById;

public sealed record GetCustomerByIdResult(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth,
    long Version
);