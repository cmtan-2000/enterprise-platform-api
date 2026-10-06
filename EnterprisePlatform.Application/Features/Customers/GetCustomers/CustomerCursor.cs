namespace EnterprisePlatform.Application.Features.Customers.GetCustomers;

public sealed record CustomerCursor(
    DateTime CreatedAt,
    Guid Id);