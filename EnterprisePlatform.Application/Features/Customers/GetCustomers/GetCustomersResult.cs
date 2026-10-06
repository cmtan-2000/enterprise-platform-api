namespace EnterprisePlatform.Application.Features.Customers.GetCustomers;

public sealed record GetCustomersResult(
    IReadOnlyList<CustomerListItem> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

public sealed record CustomerListItem(
    Guid Id,
    string FirstName,
    string LastName,
    string Email);