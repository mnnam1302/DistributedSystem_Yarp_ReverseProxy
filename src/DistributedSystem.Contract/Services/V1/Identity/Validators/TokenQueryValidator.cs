using FluentValidation;

namespace DistributedSystem.Contract.Services.V1.Identity.Validators;

public class TokenQueryValidator : AbstractValidator<Query.TokenQuery>
{
    public TokenQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.AccessToken)
            .NotEmpty()
            .MinimumLength(10);

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MinimumLength(10);
    }
}
