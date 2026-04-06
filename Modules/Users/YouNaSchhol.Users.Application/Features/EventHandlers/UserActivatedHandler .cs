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
    public sealed class UserActivatedHandler : INotificationHandler<UserActivatedEvent>
    {
        private readonly IRabbitMqPublisher _publisher;
        private readonly ILogger<UserActivatedHandler> _logger;

        public UserActivatedHandler(IRabbitMqPublisher publisher, ILogger<UserActivatedHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(UserActivatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Handling UserActivatedEvent for User {UserId}", domainEvent.UserId);

            var integrationEvent = new
            {
                domainEvent.UserId,
                domainEvent.OccurredOn
            };

            string payload = JsonSerializer.Serialize(integrationEvent);

            await _publisher.PublishAsync(
                routingKey: "user.activated",
                messageType: nameof(UserActivatedEvent),
                messageBody: payload,
                cancellationToken: cancellationToken
            );
        }
    }
}
