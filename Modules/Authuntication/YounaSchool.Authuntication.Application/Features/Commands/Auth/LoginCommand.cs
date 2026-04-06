using MediatR;
using Shared.Application.RESULT_PATTERN;
using System.Net;
using System.Security.Claims;
using YounaSchool.Authuntication.Application.Abstractions.Persistence;
using YounaSchool.Authuntication.Application.Abstractions.Security;
using YounaSchool.Authuntication.Application.DTOs;
using YounaSchool.Authuntication.Domain.Aggregates;
using YounaSchool.Authuntication.Domain.Interfaces.Repositories;
using SharedKernel.Application.UNIT_OF_WORK;
using YounaSchool.Authentication.Domain.ValueObjects;

namespace YounaSchool.Authuntication.Application.Features.Commands.Auth;

public sealed class LoginCommand : IRequest<Result<AuthResponseDto>>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string? DeviceInfo { get; init; }
    public string? IpAddress { get; init; }
}

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly IAuthSessionRepository _sessionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IApplicationUserRepository userRepository,
        IUserCredentialRepository credentialRepository,
        IAuthSessionRepository sessionRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _credentialRepository = credentialRepository;
        _sessionRepository = sessionRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null)
        {
            return Result<AuthResponseDto>.Failure("Invalid credentials.", HttpStatusCode.Unauthorized);
        }

        if (!user.IsActive)
        {
            return Result<AuthResponseDto>.Failure("Account is not active.", HttpStatusCode.Unauthorized);
        }

        if (!user.EmailConfirmed)
        {
            return Result<AuthResponseDto>.Failure("Email address is not confirmed.", HttpStatusCode.Unauthorized);
        }

        var credential = await _credentialRepository.GetByUserIdAsync(user.Id, true, cancellationToken);
        if (credential is null)
        {
            return Result<AuthResponseDto>.Failure("Invalid credentials.", HttpStatusCode.Unauthorized);
        }

        if (!_passwordHasher.Verify(request.Password, credential.Password.Value))
        {
            return Result<AuthResponseDto>.Failure("Invalid credentials.", HttpStatusCode.Unauthorized);
        }

        var rawRefreshToken = _tokenGenerator.GenerateRefreshToken();
        var refreshTokenHash = _passwordHasher.Hash(rawRefreshToken);
        var refreshVoResult = RefreshToken.Create(refreshTokenHash, DateTime.UtcNow.AddDays(7));
        if (!refreshVoResult.IsSuccess || refreshVoResult.Value is null)
        {
            return Result<AuthResponseDto>.Failure(refreshVoResult.Error ?? "Unable to create refresh token.", HttpStatusCode.BadRequest);
        }

        var sessionResult = AuthSession.Create(user.Id, refreshVoResult.Value, request.DeviceInfo, request.IpAddress);
        if (!sessionResult.IsSuccess || sessionResult.Value is null)
        {
            return Result<AuthResponseDto>.Failure(sessionResult.Error ?? "Unable to create auth session.", HttpStatusCode.BadRequest);
        }

        await _sessionRepository.AddAsync(sessionResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Role, user.Role)
        };

        var accessToken = _tokenGenerator.GenerateAccessToken(claims);

        var response = new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            SessionId = sessionResult.Value.Id,
            Message = "Login succeeded."
        };

        return Result<AuthResponseDto>.Success(response);
    }
}
