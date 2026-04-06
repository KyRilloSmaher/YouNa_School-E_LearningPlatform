using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Enums;
using YouNaSchool.Notifications.Domain.Interfaces.Repositories;
using YouNaSchool.Notifications.Domain.Models;

namespace YouNaSchool.Notifications.Application.IntegrationEvents.Handlers
{
    public class CreateDefaultNotificationPreferencesHandler : IIntegrationEventHandler<UserCreatedIntegrationEvent>
    {
        private readonly INotificationPreferenceRepository _preferenceRepo;

        public CreateDefaultNotificationPreferencesHandler(INotificationPreferenceRepository preferenceRepo)
        {
            _preferenceRepo = preferenceRepo;
        }

        public async Task HandleAsync(UserCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            var defaultPreferences = new[]
            {
            new NotificationPreference(@event.UserId, NotificationChannel.Email, true),
            new NotificationPreference(@event.UserId, NotificationChannel.Push, true),
            new NotificationPreference(@event.UserId, NotificationChannel.SMS, false)
            };

            foreach (var pref in defaultPreferences)
            {
                await _preferenceRepo.AddAsync(pref, cancellationToken);
            }
        }
    }
}
