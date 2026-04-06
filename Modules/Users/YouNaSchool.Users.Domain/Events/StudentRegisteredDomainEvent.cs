/// <summary>
/// Domain event raised when a student completes registration
/// This is internal to the Users module and triggers the integration event
/// </summary>
public sealed record StudentRegisteredDomainEvent
{
    public Guid StudentId { get; init; }
    public string Email { get; init; } = null!;
    public string FullName { get; init; } = null!;
    public DateTime RegisteredAt { get; init; }

    public StudentRegisteredDomainEvent(
        Guid studentId,
        string email,
        string fullName,
        DateTime registeredAt)
    {
        StudentId = studentId;
        Email = email;
        FullName = fullName;
        RegisteredAt = registeredAt;
    }
}