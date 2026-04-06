using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.RESULT_PATTERN;
using YouNaSchool.Notifications.Application.DTO;
using YouNaSchool.Notifications.Application.Features.Queries;
using YouNaSchool.Notifications.Domain.Interfaces.Repositories;
using YouNaSchool.Notifications.Domain.Models;

public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery,Result<IEnumerable<NotificationDto>>>
{
    private readonly INotificationRepository _notificationRepo;

    public GetUserNotificationsQueryHandler(INotificationRepository notificationRepo)
    {
        _notificationRepo = notificationRepo;
    }

    public async Task<Result<IEnumerable<NotificationDto>>> Handle(GetUserNotificationsQuery query, CancellationToken ct)
    {
        var notifications = await _notificationRepo.GetUnreadByUserAsync(query.UserId, ct);

        var dtos = notifications.Select(n => new NotificationDto(
            n.Id,
            n.Title,
            n.Message,
            n.Channel,
            n.IsRead
        ));
        return Result<IEnumerable<NotificationDto>>.Success(dtos);
    }
}