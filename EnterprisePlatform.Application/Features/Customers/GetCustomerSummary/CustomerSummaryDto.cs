namespace EnterprisePlatform.Application.Features.Customers.GetCustomerSummary;

public sealed record CustomerSummaryDto(
    Guid Id,
    string FirstName,
    string LastName,
    int OrderCount,
    int AddressCount);