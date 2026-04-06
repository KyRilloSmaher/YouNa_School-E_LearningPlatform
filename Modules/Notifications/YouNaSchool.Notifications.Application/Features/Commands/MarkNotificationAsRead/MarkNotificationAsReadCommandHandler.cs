using MediatR;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.RESULT_PATTERN;
using SharedKernel.Application.UNIT_OF_WORK;
using YouNaSchool.Notifications.Application.Features.Commands.MarkNotificationAsRead;
using YouNaSchool.Notifications.Domain.Interfaces.Repositories;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Result<bool>>
{
    private readonly INotificationRepository _notificationRepo;
    private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepo, IUnitOfWork unitOfWork)
    {
        _notificationRepo = notificationRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(MarkNotificationAsReadCommand command, CancellationToken ct)
    {
        var notification = await _notificationRepo.GetByIdAsync(command.NotificationId, ct);
        if (notification == null) throw new Exception("Notification not found");

        notification.MarkAsRead();
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}