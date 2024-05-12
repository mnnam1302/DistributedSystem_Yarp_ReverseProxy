using FluentValidation;

namespace DistributedSystem.Contract.Services.V1.Identity.Validators;

public class LogoutValidator : AbstractValidator<Command.LogoutCommand>
{
    public LogoutValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.AccessToken)
            .NotEmpty();
    }
}
