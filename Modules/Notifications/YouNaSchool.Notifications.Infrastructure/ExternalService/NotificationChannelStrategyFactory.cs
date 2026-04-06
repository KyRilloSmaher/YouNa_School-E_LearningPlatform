using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Enums;

namespace YouNaSchool.Notifications.Infrastructure.ExternalService
{

    public class NotificationChannelStrategyFactory : INotificationChannelStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationChannelStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public INotificationChannelStrategy GetStrategy(NotificationChannel channel)
        {
            return channel switch
            {
                NotificationChannel.Email => _serviceProvider.GetRequiredService<EmailNotificationStrategy>(),
                NotificationChannel.SMS => _serviceProvider.GetRequiredService<SmsNotificationStrategy>(),
                NotificationChannel.Push => _serviceProvider.GetRequiredService<PushNotificationStrategy>(),
                _ => throw new NotSupportedException($"Channel {channel} is not supported")
            };
        }
    }
}
