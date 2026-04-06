using MediatR;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.RESULT_PATTERN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Notifications.Application.Features.Commands.MarkNotificationAsRead
{
        public record MarkNotificationAsReadCommand(Guid NotificationId) : IRequest<Result<bool>>;
}
