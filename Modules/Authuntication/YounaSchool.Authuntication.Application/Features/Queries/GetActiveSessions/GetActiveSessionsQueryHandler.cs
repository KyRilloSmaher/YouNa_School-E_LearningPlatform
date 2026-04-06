using AutoMapper;
using MediatR;
using Shared.Application.RESULT_PATTERN;
using YounaSchool.Authuntication.Application.DTOs;
using YounaSchool.Authuntication.Domain.Interfaces.Repositories;

namespace YounaSchool.Authuntication.Application.Features.Queries.GetActiveSessions;

internal sealed class GetActiveSessionsQueryHandler : IRequestHandler<GetActiveSessionsQuery, Result<IReadOnlyList<SessionDto>>>
{
    private readonly IAuthSessionRepository _sessionRepository;
    private readonly IMapper _mapper;

    public GetActiveSessionsQueryHandler(IAuthSessionRepository sessionRepository, IMapper mapper)
    {
        _sessionRepository = sessionRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<SessionDto>>> Handle(
        GetActiveSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var sessions = await _sessionRepository.GetActiveSessionsForUserAsync(request.UserId, cancellationToken);
        var dtos = _mapper.Map<IReadOnlyList<SessionDto>>(sessions);
        return Result<IReadOnlyList<SessionDto>>.Success(dtos);
    }
}
