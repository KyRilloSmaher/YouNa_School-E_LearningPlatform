using FluentValidation;
using YounaSchool.Authuntication.Application.Features.Queries.GetUserProfile;

namespace YounaSchool.Authuntication.Application.Validators;

public sealed class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
