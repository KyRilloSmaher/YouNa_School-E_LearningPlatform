using MediatR;
using Shared.Application.RESULT_PATTERN;
using System.Net;
using YounaSchool.Authuntication.Domain.Interfaces.Repositories;
using SharedKernel.Application.UNIT_OF_WORK;
using SharedKernel.Application.RESULT_PATTERN;

namespace YounaSchool.Authuntication.Application.Features.Commands.Auth;

public sealed class LogoutCommand : IRequest<Result>
{
    public Guid SessionId { get; init; }
    public string Reason { get; init; } = "User logout";
}

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IAuthSessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(
        IAuthSessionRepository sessionRepository,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, true, cancellationToken);
        if (session is null)
        {
            return Result.Failure("Session not found.");
        }

        var revokeResult = session.Revoke(request.Reason);
        if (!revokeResult.IsSuccess)
        {
            return Result.Failure(revokeResult.Error ?? "Unable to revoke session.");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

