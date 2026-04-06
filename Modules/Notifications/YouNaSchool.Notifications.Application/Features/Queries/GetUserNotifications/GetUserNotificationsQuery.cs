using MediatR;
using Shared.Application.RESULT_PATTERN;
using YouNaSchool.Notifications.Application.DTO;

namespace YouNaSchool.Notifications.Application.Features.Queries
{
    public record GetUserNotificationsQuery(Guid UserId) : IRequest<Result<IEnumerable<NotificationDto>>>;
}