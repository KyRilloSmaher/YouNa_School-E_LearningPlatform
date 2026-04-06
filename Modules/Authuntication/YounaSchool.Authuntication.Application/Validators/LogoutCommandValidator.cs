using FluentValidation;
using YounaSchool.Authuntication.Application.Features.Commands.Auth;

namespace YounaSchool.Authuntication.Application.Validators;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");
    }
}
