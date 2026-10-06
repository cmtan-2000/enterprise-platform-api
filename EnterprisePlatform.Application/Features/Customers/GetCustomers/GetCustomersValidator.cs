using FluentValidation;

namespace EnterprisePlatform.Application.Features.Customers.GetCustomers;

public sealed class GetCustomersValidator
    : AbstractValidator<GetCustomersQuery>
{
    private static readonly string[] AllowedSortFields =
    [
        "firstName",
        "lastName",
        "email",
        "dateOfBirth"
    ];

    public GetCustomersValidator()
    {
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }

    private static bool BeValidSortDirection(
        string? sortDirection)
    {
        return sortDirection is "asc" or "desc";
    }

    private static bool BeValidSortField(
        string? sortBy)
    {
        return AllowedSortFields.Contains(
            sortBy,
            StringComparer.OrdinalIgnoreCase);
    }
}