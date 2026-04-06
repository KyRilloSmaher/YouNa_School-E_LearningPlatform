using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Enums;

namespace YouNaSchool.Notifications.Infrastructure.ExternalService
{
    // Factory to get the appropriate strategy
    public interface INotificationChannelStrategyFactory
    {
        INotificationChannelStrategy GetStrategy(NotificationChannel channel);
    }
}
