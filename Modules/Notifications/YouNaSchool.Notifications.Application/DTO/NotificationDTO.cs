using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Enums;

namespace YouNaSchool.Notifications.Application.DTO
{
    public record NotificationDto(
    Guid Id,
    string Title,
    string Message,
    NotificationChannel Channel,
    bool IsRead
);
}
