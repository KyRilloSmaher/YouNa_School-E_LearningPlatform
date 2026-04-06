using Auth.Application.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using YounaSchool.Authuntication.Application.Abstractions.Messaging;
using YounaSchool.Authuntication.Application.IntegrationEvents;
using YounaSchool.Authuntication.Domain.Events;

namespace YounaSchool.Authuntication.Application.Features.EventHandlers;

internal sealed class UserActivatedEventHandler : INotificationHandler<UserActivatedEvent>
{
    private readonly IIntegrationEventPublisher _publisher;
    private readonly ILogger<UserActivatedEventHandler> _logger;

    public UserActivatedEventHandler(
        IIntegrationEventPublisher publisher,
        ILogger<UserActivatedEventHandler> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Handle(UserActivatedEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new UserActivatedInIntegrationEvent(notification.UserId,notification.Email);
        await _publisher.PublishAsync(integrationEvent, cancellationToken);

        _logger.LogInformation(
            "Published {Event} for UserId {UserId}",
            nameof(UserActivatedInIntegrationEvent),
            notification.UserId);
    }
}
