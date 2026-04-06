using Microsoft.Extensions.Logging;
using Shared.Application.IntegrationEvents;
using SharedKernel.Application.UNIT_OF_WORK;
using YounaSchool.Authuntication.Application.IntegrationEvents;
using YounaSchool.Users.Domain.Enums;
using YouNaSchhol.Users.Application.IntegrationEvents;
using YouNaSchool.Users.Domain.Entities;
using YouNaSchool.Users.Application.Abstractions.Persistence;
using Shared.Application.IntegrationEvent;

namespace YouNaSchool.Users.Infrastructure.IntegrationEvents.IntegrationEventsHandlers;

public sealed class AuthUserConfirmedIntegrationEventHandler
    : IIntegrationEventHandler<AuthUserConfirmedIntegrationEvent>
{
    private readonly IStudentRepository _studentRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IAssistantRepository _assistantRepository;
    private readonly IIntegrationEventPublisher _publisher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthUserConfirmedIntegrationEventHandler> _logger;

    public AuthUserConfirmedIntegrationEventHandler(
        IStudentRepository studentRepository,
        ITeacherRepository teacherRepository,
        IAssistantRepository assistantRepository,
        IIntegrationEventPublisher publisher,
        IUnitOfWork unitOfWork,
        ILogger<AuthUserConfirmedIntegrationEventHandler> logger)
    {
        _studentRepository = studentRepository;
        _teacherRepository = teacherRepository;
        _assistantRepository = assistantRepository;
        _publisher = publisher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(AuthUserConfirmedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling confirmed auth user {UserId} for role {Role}",
            @event.UserId,
            @event.Role);

        if (!Enum.TryParse<UserRole>(@event.Role, true, out var role))
        {
            role = UserRole.Student;
        }

        switch (role)
        {
            case UserRole.Student:
                await EnsureStudentAsync(@event, cancellationToken);
                break;
            case UserRole.Teacher:
                await EnsureTeacherAsync(@event.UserId, cancellationToken);
                break;
            case UserRole.Assistant:
                await EnsureAssistantAsync(@event.UserId, cancellationToken);
                break;
            default:
                await EnsureStudentAsync(@event, cancellationToken);
                break;
        }
    }

    private async Task EnsureStudentAsync(AuthUserConfirmedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        var existingStudent = await _studentRepository.GetByUserIdAsync(@event.UserId, false, cancellationToken);
        if (existingStudent is not null)
        {
            _logger.LogInformation("Student profile already exists for user {UserId}", @event.UserId);
            return;
        }

        var student = Student.Create(@event.UserId, Level.First);
        await _studentRepository.AddAsync(student, cancellationToken);

        await _publisher.PublishAsync(
            new StudentCreatedIntegrationEvent(
                @event.UserId,
                @event.Email,
                $"{@event.FirstName} {@event.LastName}".Trim(),
                DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureTeacherAsync(Guid userId, CancellationToken cancellationToken)
    {
        var existingTeacher = await _teacherRepository.GetByUserIdAsync(userId, false, cancellationToken);
        if (existingTeacher is not null)
        {
            return;
        }

        await _teacherRepository.AddAsync(Teacher.Create(userId), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureAssistantAsync(Guid userId, CancellationToken cancellationToken)
    {
        var existingAssistant = await _assistantRepository.GetByUserIdAsync(userId, false, cancellationToken);
        if (existingAssistant is not null)
        {
            return;
        }

        await _assistantRepository.AddAsync(Assistant.Create(userId, Guid.Empty), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
