using MediatR;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.RESULT_PATTERN;
using SharedKernel.Application.UNIT_OF_WORK;
using YouNaSchool.Notifications.Application.Features.Commands.Create;
using YouNaSchool.Notifications.Domain.Interfaces.Repositories;
using YouNaSchool.Notifications.Domain.Models;

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Result<Guid>>
{
    private readonly INotificationRepository _notificationRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNotificationCommandHandler(INotificationRepository notificationRepo, IUnitOfWork unitOfWork)
    {
        _notificationRepo = notificationRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateNotificationCommand command, CancellationToken ct)
    {
        var notification = Notification.Create(command.UserId, command.Channel, command.Title, command.Message ,command.phone ,command.Email);
        await _notificationRepo.AddAsync(notification, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<Guid>.Success(notification.Id);
    }
}