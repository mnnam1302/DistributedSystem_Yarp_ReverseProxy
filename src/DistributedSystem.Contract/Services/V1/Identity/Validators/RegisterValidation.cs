using FluentValidation;

namespace DistributedSystem.Contract.Services.V1.Identity.Validators
{
    public class RegisterValidation : AbstractValidator<Command.RegisterCommand>
    {
        public RegisterValidation()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotEmpty();
            RuleFor(x => x.PhoneNumber).NotEmpty();

            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}