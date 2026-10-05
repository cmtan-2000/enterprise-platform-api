namespace EnterprisePlatform.Application.Features.Customers.CreateCustomer;

public sealed record CreateCustomerResult(
    Guid Id,
    string FirstName,
    string LastName,
    string Email);