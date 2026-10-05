using FluentValidation;

namespace EnterprisePlatform.Application.Features.Customers.UpdateCustomer;

public sealed class UpdateCustomerValidator
    : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30)
            .When(x => x.PhoneNumber is not null);

        RuleFor(x => x.ExpectedVersion)
            .GreaterThanOrEqualTo(0);
    }
}