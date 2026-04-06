
using YouNaSchool.Notifications.Application.IntegrationEvents;
using YouNaSchool.Notifications.Domain.Enums;
using YouNaSchool.Notifications.Domain.Interfaces.Repositories;
using YouNaSchool.Notifications.Domain.Models;

public class SendDeactivateAccountNotificationHandler : IIntegrationEventHandler<UserDeactivatedIntegrationEvent>
{
    private readonly INotificationRepository _notificationRepo;
    private readonly INotificationPreferenceRepository _preferenceRepo;

    public SendDeactivateAccountNotificationHandler(
        INotificationRepository notificationRepo,
        INotificationPreferenceRepository preferenceRepo)
    {
        _notificationRepo = notificationRepo;
        _preferenceRepo = preferenceRepo;
    }

    public async Task HandleAsync(UserDeactivatedIntegrationEvent evt, CancellationToken ct)
    {
        // Check user preference
        var isEmailEnabled = await _preferenceRepo.IsEnabledAsync(evt.UserId, NotificationChannel.Email, ct);
        if (!isEmailEnabled) return;

        var notification = Notification.Create(
            evt.UserId,
            NotificationChannel.Email,
            "Account Deactivated",
            $"Your account has been Deactivated at {evt.DeactivatedAt:yyyy-MM-dd HH:mm}.",
            evt.phone,
            evt.Email

        );

        await _notificationRepo.AddAsync(notification, ct);
    }
}