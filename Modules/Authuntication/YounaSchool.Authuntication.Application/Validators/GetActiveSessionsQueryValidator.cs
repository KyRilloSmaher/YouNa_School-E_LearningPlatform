using FluentValidation;
using YounaSchool.Authuntication.Application.Features.Queries.GetActiveSessions;

namespace YounaSchool.Authuntication.Application.Validators;

public sealed class GetActiveSessionsQueryValidator : AbstractValidator<GetActiveSessionsQuery>
{
    public GetActiveSessionsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
