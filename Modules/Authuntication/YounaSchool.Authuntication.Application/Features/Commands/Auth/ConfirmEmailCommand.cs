using MediatR;
using Shared.Application.RESULT_PATTERN;
using System.Net;
using SharedKernel.Application.UNIT_OF_WORK;
using YounaSchool.Authuntication.Application.Abstractions.Messaging;
using YounaSchool.Authuntication.Application.Abstractions.Persistence;
using YounaSchool.Authuntication.Application.IntegrationEvents;

namespace YounaSchool.Authuntication.Application.Features.Commands.Auth;

public sealed record ConfirmEmailCommand(string Email, string Token) : IRequest<Result<bool>>;

public sealed class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<bool>>
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly IIntegrationEventPublisher _eventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmEmailCommandHandler(
        IApplicationUserRepository userRepository,
        IIntegrationEventPublisher eventPublisher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _eventPublisher = eventPublisher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null)
        {
            return Result<bool>.Failure("User not found.", HttpStatusCode.NotFound);
        }

        if (user.EmailConfirmed)
        {
            return Result<bool>.Success(true);
        }

        if (string.IsNullOrWhiteSpace(user.ConfirmationToken) ||
            !string.Equals(user.ConfirmationToken, request.Token, StringComparison.Ordinal))
        {
            return Result<bool>.Failure("Invalid confirmation token.", HttpStatusCode.BadRequest);
        }

        if (user.ConfirmationTokenExpiresAtUtc is null || user.ConfirmationTokenExpiresAtUtc <= DateTime.UtcNow)
        {
            return Result<bool>.Failure("Confirmation token has expired.", HttpStatusCode.BadRequest);
        }

        await _userRepository.MarkEmailConfirmedAsync(user.Id, cancellationToken);

        await _eventPublisher.PublishAsync(
            new AuthUserConfirmedIntegrationEvent(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Role),
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
