using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YouNaSchhol.Users.Application.Abstractions.Messaging.RabbitMq;
using YounaSchool.Users.Domain.Events;

namespace YouNaSchhol.Users.Application.Features.EventHandlers
{
    public sealed class UserDeactivatedHandler : INotificationHandler<UserDeactivatedEvent>
    {
        private readonly IRabbitMqPublisher _publisher;
        private readonly ILogger<UserDeactivatedHandler> _logger;

        public UserDeactivatedHandler(IRabbitMqPublisher publisher, ILogger<UserDeactivatedHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(UserDeactivatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Handling UserDeactivatedEvent for User {UserId}", domainEvent.UserId);

            var integrationEvent = new
            {
                domainEvent.UserId,
                domainEvent.Reason,
                domainEvent.OccurredOn
            };

            string payload = JsonSerializer.Serialize(integrationEvent);

            await _publisher.PublishAsync(
                routingKey: "user.deactivated",
                messageType: nameof(UserDeactivatedEvent),
                messageBody: payload,
                cancellationToken: cancellationToken
            );
        }
    }
}
