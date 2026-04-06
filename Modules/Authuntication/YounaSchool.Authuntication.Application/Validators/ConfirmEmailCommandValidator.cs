using FluentValidation;
using YounaSchool.Authuntication.Application.Features.Commands.Auth;

namespace YounaSchool.Authuntication.Application.Validators;

public sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Token)
            .NotEmpty();
    }
}
