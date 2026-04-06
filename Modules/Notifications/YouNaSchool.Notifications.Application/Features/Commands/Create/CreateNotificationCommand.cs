using MediatR;
using Shared.Application.RESULT_PATTERN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Enums;

namespace YouNaSchool.Notifications.Application.Features.Commands.Create
{
    public record CreateNotificationCommand(
      Guid UserId,
      string Title,
      string Message,
      NotificationChannel Channel, string? phone, string? Email
  ) : IRequest<Result<Guid>>; 
}
