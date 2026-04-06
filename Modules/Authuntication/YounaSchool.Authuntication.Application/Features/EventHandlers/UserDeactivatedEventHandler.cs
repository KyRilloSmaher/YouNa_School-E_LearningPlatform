using MediatR;
using Microsoft.Extensions.Logging;
using YounaSchool.Authuntication.Application.Abstractions.Messaging;
using YounaSchool.Authuntication.Application.IntegrationEvents;
using YounaSchool.Authuntication.Domain.Events;

namespace YounaSchool.Authuntication.Application.Features.EventHandlers;

internal sealed class UserDeactivatedEventHandler : INotificationHandler<UserDeactivatedEvent>
{
    private readonly IIntegrationEventPublisher _publisher;
    private readonly ILogger<UserDeactivatedEventHandler> _logger;

    public UserDeactivatedEventHandler(
        IIntegrationEventPublisher publisher,
        ILogger<UserDeactivatedEventHandler> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Handle(UserDeactivatedEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new UserAccountSuspendIntegrationEvent(
            notification.UserId,
            notification.Email.Value,
            DateTime.UtcNow
            );

        await _publisher.PublishAsync(integrationEvent, cancellationToken);

        _logger.LogInformation(
            "Published {Event} for UserId {UserId}",
            nameof(UserAccountSuspendIntegrationEvent),
            notification.UserId);
    }
}
