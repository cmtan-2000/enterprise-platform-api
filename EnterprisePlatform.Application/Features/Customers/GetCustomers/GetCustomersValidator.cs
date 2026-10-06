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
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.SortDirection)
            .Must(BeValidSortDirection)
            .When(x => !string.IsNullOrWhiteSpace(x.SortDirection));

        RuleFor(x => x.SortBy)
            .Must(BeValidSortField)
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy));
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