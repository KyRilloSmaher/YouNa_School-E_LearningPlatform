using MediatR;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.RESULT_PATTERN;
using YounaSchool.Authuntication.Application.DTOs;

namespace YounaSchool.Authuntication.Application.Features.Queries.UserEmailQuery
{
   public record GetUserByEmailQuery(string Email) : IRequest<Result<UserDto>>;
}
