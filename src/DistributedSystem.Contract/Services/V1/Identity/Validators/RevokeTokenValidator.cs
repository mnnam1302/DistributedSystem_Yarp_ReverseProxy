using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace DistributedSystem.Contract.Services.V1.Identity.Validators;
public class RevokeTokenValidator : AbstractValidator<Command.RevokeTokenCommand>
{
    public RevokeTokenValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.AccessToken)
            .NotEmpty();
    }
}
