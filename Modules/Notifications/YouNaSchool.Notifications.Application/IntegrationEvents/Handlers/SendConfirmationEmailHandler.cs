using YounaSchool.Authuntication.Application.IntegrationEvents;
using YouNaSchool.Notifications.Domain.Enums;
using YouNaSchool.Notifications.Domain.Interfaces.Repositories;
using YouNaSchool.Notifications.Domain.Interfaces.Services;
using YouNaSchool.Notifications.Domain.Models;

namespace YouNaSchool.Notifications.Application.IntegrationEvents.Handlers;

public class SendConfirmationEmailHandler : IIntegrationEventHandler<SendConfirmationEmailIntegrationEvent>
{
    private readonly INotificationRepository _notificationRepo;
    private readonly IEmailService _emailService;

    public SendConfirmationEmailHandler(
        INotificationRepository notificationRepo,
        IEmailService emailService)
    {
        _notificationRepo = notificationRepo;
        _emailService = emailService;
    }

    public async Task HandleAsync(SendConfirmationEmailIntegrationEvent evt, CancellationToken ct)
    {
        var notification = Notification.Create(
            evt.UserId,
            NotificationChannel.Email,
            evt.Title,
            $"{evt.Message}{Environment.NewLine}{Environment.NewLine}Confirm here: {evt.ConfirmationLink}",
            null,
            evt.Email);

        await _notificationRepo.AddAsync(notification, ct);
        await _emailService.SendEmailAsync(evt.Email, evt.Title, notification.Message);
    }
}
