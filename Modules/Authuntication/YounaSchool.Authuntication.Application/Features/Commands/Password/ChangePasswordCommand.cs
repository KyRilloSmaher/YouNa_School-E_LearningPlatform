using MediatR;
using Shared.Application.RESULT_PATTERN;
using System.Net;
using YounaSchool.Authentication.Domain.ValueObjects;
using YounaSchool.Authuntication.Application.Abstractions.Security;
using YounaSchool.Authuntication.Domain.Interfaces.Repositories;
using SharedKernel.Application.UNIT_OF_WORK;
using SharedKernel.Application.ClockandUserContext;
using SharedKernel.Application.RESULT_PATTERN;

namespace YounaSchool.Authuntication.Application.Features.Commands.Password;

public sealed class ChangePasswordCommand : IRequest<Result>
{
    public string CurrentPassword { get; init; } = null!;
    public string NewPassword { get; init; } = null!;
}

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(
        IUserCredentialRepository credentialRepository,
        IPasswordHasher passwordHasher,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _credentialRepository = credentialRepository;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || !Guid.TryParse(_currentUser.UserId, out var userId))
        {
            return Result.Failure("User not authenticated.");
        }

        var credential = await _credentialRepository.GetByUserIdAsync(userId, true, cancellationToken);
        if (credential is null)
        {
            return Result.Failure("Credentials not found.");
        }

        if (!_passwordHasher.Verify(request.CurrentPassword, credential.Password.Value))
        {
            return Result.Failure("Current password is incorrect.");
        }

        var newHash = _passwordHasher.Hash(request.NewPassword);
        var newPasswordResult = HashedPassword.Create(newHash);
        if (!newPasswordResult.IsSuccess || newPasswordResult.Value is null)
        {
            return Result.Failure(newPasswordResult.Error ?? "Invalid new password.");
        }

        var changeResult = credential.ChangePassword(newPasswordResult.Value);
        if (!changeResult.IsSuccess)
        {
            return Result.Failure(changeResult.Error ?? "Unable to change password.");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

