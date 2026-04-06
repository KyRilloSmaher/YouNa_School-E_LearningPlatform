using SharedKernel.Domain.CoreAbstractions;
using SharedKernel.Application.RESULT_PATTERN;

namespace YouNaSchool.Users.Domain.Entities;

/// <summary>
/// Represents an assistant assigned to a teacher.
/// An assistant supervises multiple students.
/// </summary>
public sealed class Assistant : AggregateRoot
{
    /// <summary>
    /// Primary identifier of the assistant aggregate.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Reference to the Identity user (Auth module).
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The teacher supervising this assistant.
    /// </summary>
    public Guid TeacherId { get; private set; }

    private Assistant() { }

    private Assistant(Guid id, Guid userId, Guid teacherId)
    {
        Id = id;
        UserId = userId;
        TeacherId = teacherId;
    }

    /// <summary>
    /// Factory method for creating an assistant.
    /// </summary>
    public static Assistant Create(Guid userId, Guid teacherId)
        => new(Guid.NewGuid(), userId, teacherId);

    /// <summary>
    /// Changes the supervising teacher.
    /// </summary>
    public Result ChangeSupervisor(Guid teacherId)
    {
        if (TeacherId == teacherId)
            return Result.Failure("Assistant already assigned to this teacher.");

        TeacherId = teacherId;

        return Result.Success();
    }
}
