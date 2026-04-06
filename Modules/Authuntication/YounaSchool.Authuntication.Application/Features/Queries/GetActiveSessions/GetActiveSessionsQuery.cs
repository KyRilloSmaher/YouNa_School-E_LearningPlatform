using MediatR;
using Shared.Application.RESULT_PATTERN;
using YounaSchool.Authuntication.Application.DTOs;

namespace YounaSchool.Authuntication.Application.Features.Queries.GetActiveSessions;

public sealed record GetActiveSessionsQuery(Guid UserId) : IRequest<Result<IReadOnlyList<SessionDto>>>;
