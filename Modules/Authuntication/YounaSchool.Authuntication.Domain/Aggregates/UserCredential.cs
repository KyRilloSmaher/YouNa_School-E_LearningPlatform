
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Domain.CoreAbstractions;
using YounaSchool.Authentication.Domain.ValueObjects;
using YounaSchool.Authuntication.Domain.Events;

namespace YounaSchool.Authentication.Domain.Aggregates;

public sealed class UserCredential : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }

    public HashedPassword Password { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? LastPasswordChangeUtc { get; private set; }

    /// <summary>
    /// Keeps track of the last N password hashes to prevent reuse.
    /// </summary>
    private List<string> _passwordHistory = new();

    public IReadOnlyCollection<string> PasswordHistory => _passwordHistory.AsReadOnly();

    private UserCredential() { }

    private UserCredential(Guid id, Guid userId, HashedPassword password)
    {
        Id = id;
        UserId = userId;
        Password = password;
        CreatedAtUtc = DateTime.UtcNow;
        LastPasswordChangeUtc = null;

        // Seed history with the initial password hash
        TrackPassword(password.Value);
    }

    /// <summary>
    /// Factory method to create a new user credential aggregate.
    /// </summary>
    public static Result<UserCredential> Create(Guid userId, HashedPassword password)
    {
        if (userId == Guid.Empty)
        {
            return Result<UserCredential>.Failure("UserId cannot be empty.", System.Net.HttpStatusCode.BadRequest);
        }

        if (password is null)
        {
            return Result<UserCredential>.Failure("Password is required.", System.Net.HttpStatusCode.BadRequest);
        }

        var credential = new UserCredential(Guid.NewGuid(), userId, password);
        return Result<UserCredential>.Success(credential);
    }

    /// <summary>
    /// Changes the password while enforcing simple reuse rules and raising a domain event.
    /// </summary>
    public Result<bool> ChangePassword(HashedPassword newPassword)
    {
        if (newPassword is null)
        {
            return Result<bool>.Failure("New password is required.", System.Net.HttpStatusCode.BadRequest);
        }

        // Prevent using the same or recently used password hashes
        if (newPassword.Value == Password.Value || _passwordHistory.Contains(newPassword.Value))
        {
            return Result<bool>.Failure("New password must be different from recent passwords.", System.Net.HttpStatusCode.BadRequest);
        }

        Password = newPassword;
        LastPasswordChangeUtc = DateTime.UtcNow;
        TrackPassword(newPassword.Value);

        RaiseDomainEvent(new PasswordChangedDomainEvent(UserId));

        return Result<bool>.Success(true);
    }

    private void TrackPassword(string hash)
    {
        _passwordHistory.Add(hash);
        const int MaxHistory = 3;

        if (_passwordHistory.Count > MaxHistory)
        {
            _passwordHistory.RemoveAt(0);
        }
    }
}