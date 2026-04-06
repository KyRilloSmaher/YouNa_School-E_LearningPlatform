using SharedKernel.Application.RESULT_PATTERN;
using SharedKernel.Domain.CoreAbstractions;
using YounaSchool.Users.Domain.Enums;
namespace YouNaSchool.Users.Domain.Entities;

/// <summary>
/// Represents a student within the academic system.
/// A student is linked to an identity user (Auth module)
/// and can be assigned to an assistant supervisor.
/// </summary>
public sealed class Student : AggregateRoot
{
    /// <summary>
    /// Primary identifier of the student aggregate.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Reference to the Identity user (Auth module).
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The assigned assistant responsible for supervising the student.
    /// </summary>
    public Guid? AssistantId { get; private set; }

    /// <summary>
    /// Current academic level of the student.
    /// </summary>
    public Level Level { get; private set; }

    /// <summary>
    /// Indicates whether the student is currently suspended.
    /// </summary>
    public bool IsSuspended { get; private set; }

    /// <summary>
    /// Total number of times the student has been suspended.
    /// </summary>
    public int SuspensionCount { get; private set; }

    private Student() { }

    private Student(Guid id, Guid userId, Level level)
    {
        Id = id;
        UserId = userId;
        Level = level;
        IsSuspended = false;
        SuspensionCount = 0;
    }

    /// <summary>
    /// Factory method for creating a new student.
    /// </summary>
    public static Student Create(Guid userId, Level level)
        => new(Guid.NewGuid(), userId, level);

    /// <summary>
    /// Assigns an assistant supervisor to the student.
    /// </summary>
    public Result AssignAssistant(Guid assistantId)
    {
        if (AssistantId.HasValue)
            return Result.Failure("Student already assigned to an assistant.");

        AssistantId = assistantId;

        return Result.Success();
    }
    /// <summary>
    /// Removes Assigned assistant supervisor from the student.
    /// </summary>
    public Result RemoveAssistant(Guid assistantId)
    {
        if (AssistantId.HasValue)
            return Result.Failure("Student already assigned to an assistant.");

        AssistantId = null;

        return Result.Success();
    }


    /// <summary>
    /// Suspends the student for disciplinary reasons.
    /// </summary>
    public Result Suspend(string reason)
    {
        if (IsSuspended)
            return Result.Failure("Student is already suspended.");

        IsSuspended = true;
        SuspensionCount++;

        return Result.Success();
    }

    /// <summary>
    /// Removes suspension from the student.
    /// </summary>
    public Result Reinstate()
    {
        if (!IsSuspended)
            return Result.Failure("Student is not suspended.");

        IsSuspended = false;

        return Result.Success();
    }
}
