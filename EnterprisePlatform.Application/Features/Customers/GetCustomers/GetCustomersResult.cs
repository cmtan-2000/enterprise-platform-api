namespace EnterprisePlatform.Application.Features.Customers.GetCustomers;

public sealed record GetCustomersResult(
    IReadOnlyList<CustomerListItem> Items,
    int PageSize,
    string? NextCursor);

public sealed record CustomerListItem(
    Guid Id,
    string FirstName,
    string LastName,
    string Email);