using FluentValidation;

namespace DistributedSystem.Contract.Services.V1.Identity.Validators
{
    public class LoginValidatior : AbstractValidator<Query.GetLoginQuery>
    {
        public LoginValidatior()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}