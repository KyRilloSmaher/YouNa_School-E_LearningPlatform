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
    public sealed class StudentSuspendedHandler : INotificationHandler<StudentSuspendedEvent>
    {
        private readonly IRabbitMqPublisher _publisher;
        private readonly ILogger<StudentSuspendedHandler> _logger;

        public StudentSuspendedHandler(IRabbitMqPublisher publisher, ILogger<StudentSuspendedHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(StudentSuspendedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Handling StudentSuspendedEvent for Student {StudentId}", domainEvent.StudentId);

            var integrationEvent = new
            {
                domainEvent.StudentId,
                domainEvent.Reason,
                domainEvent.OccurredOn
            };

            string payload = JsonSerializer.Serialize(integrationEvent);

            await _publisher.PublishAsync(
                routingKey: "user.student.suspended",
                messageType: nameof(StudentSuspendedEvent),
                messageBody: payload,
                cancellationToken: cancellationToken
            );
        }
    }
}
