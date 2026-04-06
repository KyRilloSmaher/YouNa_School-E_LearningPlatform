using MediatR;
using Shared.Application.RESULT_PATTERN;
using System.Net;
using System.Security.Claims;
using YounaSchool.Authuntication.Application.Abstractions.Security;
using YounaSchool.Authuntication.Application.DTOs;
using YounaSchool.Authuntication.Application.Abstractions.Persistence;
using YounaSchool.Authuntication.Domain.Interfaces.Repositories;
using YounaSchool.Authentication.Domain.ValueObjects;
using YounaSchool.Authuntication.Domain.Aggregates;
using SharedKernel.Application.UNIT_OF_WORK;

namespace YounaSchool.Authuntication.Application.Features.Commands.Auth;

public sealed class RefreshTokenCommand : IRequest<Result<AuthResponseDto>>
{
    public Guid SessionId { get; init; }
    public string RefreshToken { get; init; } = null!;
    public string? DeviceInfo { get; init; }
    public string? IpAddress { get; init; }
}

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IAuthSessionRepository _sessionRepository;
    private readonly IApplicationUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IAuthSessionRepository sessionRepository,
        IApplicationUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, true, cancellationToken);
        if (session is null)
        {
            return Result<AuthResponseDto>.Failure("Invalid session.", HttpStatusCode.Unauthorized);
        }

        if (session.RefreshToken.IsExpired())
        {
            session.MarkExpired();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<AuthResponseDto>.Failure("Refresh token expired.", HttpStatusCode.Unauthorized);
        }

        if (!_passwordHasher.Verify(request.RefreshToken, session.RefreshToken.TokenHash))
        {
            return Result<AuthResponseDto>.Failure("Invalid refresh token.", HttpStatusCode.Unauthorized);
        }

        // Generate new refresh token and session
        var rawRefreshToken = _tokenGenerator.GenerateRefreshToken();
        var refreshTokenHash = _passwordHasher.Hash(rawRefreshToken);
        var refreshVoResult = RefreshToken.Create(refreshTokenHash, DateTime.UtcNow.AddDays(7));
        if (!refreshVoResult.IsSuccess || refreshVoResult.Value is null)
        {
            return Result<AuthResponseDto>.Failure(refreshVoResult.Error ?? "Unable to create refresh token.", HttpStatusCode.BadRequest);
        }

        // Revoke old session
        session.Revoke("Rotated refresh token");

        var newSessionResult = AuthSession.Create(session.UserId, refreshVoResult.Value, request.DeviceInfo, request.IpAddress);
        if (!newSessionResult.IsSuccess || newSessionResult.Value is null)
        {
            return Result<AuthResponseDto>.Failure(newSessionResult.Error ?? "Unable to create auth session.", HttpStatusCode.BadRequest);
        }

        await _sessionRepository.AddAsync(newSessionResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Load user for full claims
        var user = await _userRepository.GetByIdAsync(session.UserId, cancellationToken);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, session.UserId.ToString())
        };
        if (user is not null)
        {
            claims.Add(new(ClaimTypes.Email, user.Email));
            claims.Add(new(ClaimTypes.GivenName, user.FirstName));
            claims.Add(new(ClaimTypes.Surname, user.LastName));
            claims.Add(new(ClaimTypes.Role, user.Role));
        }

        var accessToken = _tokenGenerator.GenerateAccessToken(claims);

        var response = new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            SessionId = newSessionResult.Value.Id
        };

        return Result<AuthResponseDto>.Success(response);
    }
}

