using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Application.RESULT_PATTERN;
using System.Net;
using System.Security.Cryptography;
using YounaSchool.Authuntication.Application.Abstractions.Messaging;
using YounaSchool.Authuntication.Application.Abstractions.Persistence;
using YounaSchool.Authuntication.Application.Abstractions.Security;
using YounaSchool.Authuntication.Application.DTOs;
using YounaSchool.Authuntication.Application.IntegrationEvents;
using YounaSchool.Authuntication.Domain.Aggregates;
using YounaSchool.Authuntication.Domain.Enums;
using YounaSchool.Authuntication.Domain.Interfaces.Repositories;
using SharedKernel.Application.UNIT_OF_WORK;
using YounaSchool.Authentication.Domain.ValueObjects;
using YounaSchool.Authentication.Domain.Aggregates;

namespace YounaSchool.Authuntication.Application.Features.Commands.Auth;

public sealed class RegisterCommand : IRequest<Result<AuthResponseDto>>
{
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Password { get; init; } = null!;
    public UserRole Role { get; init; } = UserRole.Student;
}

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly IAuthSessionRepository _sessionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIntegrationEventPublisher _eventPublisher;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IApplicationUserRepository userRepository,
        IUserCredentialRepository credentialRepository,
        IAuthSessionRepository sessionRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork,
        IIntegrationEventPublisher eventPublisher,
        ILogger<RegisterCommandHandler> logger)
    {
        _userRepository = userRepository;
        _credentialRepository = credentialRepository;
        _sessionRepository = sessionRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            return Result<AuthResponseDto>.Failure("Email already registered.", HttpStatusCode.Conflict);
        }

        var passwordHash = _passwordHasher.Hash(request.Password);
        var confirmationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var confirmationTokenExpiresAtUtc = DateTime.UtcNow.AddHours(24);

        var userId = await _userRepository.AddAsync(
            new CreateUserRequest(
                email,
                request.FirstName.Trim(),
                request.LastName.Trim(),
                request.Role.ToString(),
                passwordHash,
                confirmationToken,
                confirmationTokenExpiresAtUtc),
            cancellationToken);

        var passwordVoResult = HashedPassword.Create(passwordHash);
        if (!passwordVoResult.IsSuccess || passwordVoResult.Value is null)
        {
            return Result<AuthResponseDto>.Failure(passwordVoResult.Error ?? "Invalid password.", HttpStatusCode.BadRequest);
        }

        var credentialResult = UserCredential.Create(userId, passwordVoResult.Value);
        if (!credentialResult.IsSuccess || credentialResult.Value is null)
        {
            return Result<AuthResponseDto>.Failure(credentialResult.Error ?? "Unable to create credentials.", HttpStatusCode.BadRequest);
        }

        await _credentialRepository.AddAsync(credentialResult.Value, cancellationToken);

        var confirmationLink =
            $"/api/v1/Auth/confirm-email?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(confirmationToken)}";

        await _eventPublisher.PublishAsync(new SendConfirmationEmailIntegrationEvent(
            userId,
            email,
            "Confirm your account",
            $"Please confirm your email address to activate your account. Confirmation link: {confirmationLink}",
            confirmationLink), cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto
        {
            Message = "Registration completed. Please confirm your email before logging in."
        };

        return Result<AuthResponseDto>.Success(response);
    }
}
