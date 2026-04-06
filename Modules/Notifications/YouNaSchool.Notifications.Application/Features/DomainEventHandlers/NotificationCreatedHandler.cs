using MediatR;
using YouNaSchool.Notifications.Domain.Domain_Events.YouNaSchool.Notifications.Domain.Events;
using YouNaSchool.Notifications.Domain.Interfaces.Repositories;
using YouNaSchool.Notifications.Infrastructure.ExternalService;

namespace YouNaSchool.Notifications.Application.Handlers;

public class NotificationCreatedHandler : INotificationHandler<NotificationCreatedEvent>
{
    private readonly INotificationPreferenceRepository _preferenceRepo;
    private readonly INotificationChannelStrategy _channelStrategy;

    public NotificationCreatedHandler(
        INotificationPreferenceRepository preferenceRepo,
        INotificationChannelStrategy channelStrategy)
    {
        _preferenceRepo = preferenceRepo;
        _channelStrategy = channelStrategy;
    }

    public async Task Handle(NotificationCreatedEvent domainEvent, CancellationToken ct)
    {
        // Validate event
        if (domainEvent == null)
            throw new ArgumentNullException(nameof(domainEvent));

        if (string.IsNullOrWhiteSpace(domainEvent.Message))
            return;

        // Check user preferences
        var isAllowed = await _preferenceRepo.IsEnabledAsync(domainEvent.UserId, domainEvent.Channel, ct);
        if (!isAllowed)
        {
            // Log that notification was blocked by user preferences
            return;
        }

        try
        {
            // Send notification using the appropriate channel strategy
            await _channelStrategy.SendAsync(domainEvent, ct);
        }
        catch (Exception ex)
        {
            // Log the exception
            // Consider implementing a retry policy or dead-letter queue
            throw;
        }
    }
}









