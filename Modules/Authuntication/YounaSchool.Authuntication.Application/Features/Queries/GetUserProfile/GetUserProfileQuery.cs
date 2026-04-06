using MediatR;
using Shared.Application.RESULT_PATTERN;
using YounaSchool.Authuntication.Application.DTOs;

namespace YounaSchool.Authuntication.Application.Features.Queries.GetUserProfile;

public sealed record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserDto>>;
