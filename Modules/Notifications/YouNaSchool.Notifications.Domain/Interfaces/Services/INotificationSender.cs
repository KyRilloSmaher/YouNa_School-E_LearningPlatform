using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Models;

namespace YouNaSchool.Notifications.Domain.Interfaces.Services
{
    public interface INotificationSender
    {
        Task SendAsync(Guid NotificationId,Guid UserId,string Title,string Message, CancellationToken ct);
    }
}
