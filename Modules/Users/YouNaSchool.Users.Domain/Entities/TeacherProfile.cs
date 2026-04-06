using SharedKernel.Domain.CoreAbstractions;
using SharedKernel.Application.RESULT_PATTERN;

namespace YouNaSchool.Users.Domain.Entities;

/// <summary>
/// Represents a teacher within the system.
/// A teacher can supervise multiple assistants.
/// </summary>
public sealed class Teacher : AggregateRoot
{
    /// <summary>
    /// Primary identifier of the teacher aggregate.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Reference to the Identity user (Auth module).
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Indicates whether the teacher account has been verified.
    /// </summary>
    public bool IsVerified { get; private set; }

    private Teacher() { }

    private Teacher(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
        IsVerified = false;
    }

    /// <summary>
    /// Factory method for creating a teacher.
    /// </summary>
    public static Teacher Create(Guid userId)
        => new(Guid.NewGuid(), userId);

    /// <summary>
    /// Marks the teacher as verified.
    /// </summary>
    public Result Verify()
    {
        if (IsVerified)
            return Result.Failure("Teacher is already verified.");

        IsVerified = true;

        return Result.Success();
    }
}
