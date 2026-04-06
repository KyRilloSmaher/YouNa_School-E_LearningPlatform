using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YouNaSchhol.Users.Application.Abstractions.Messaging.RabbitMq;
using YounaSchool.Users.Domain.Events;

namespace YouNaSchhol.Users.Application.Features.EventHandlers
{
    public sealed class StudentAssignedToAssistantHandler : INotificationHandler<StudentAssignedToAssistantDomainEvent>
    {
        private readonly IRabbitMqPublisher _publisher;
        private readonly ILogger<StudentAssignedToAssistantHandler> _logger;

        public StudentAssignedToAssistantHandler(IRabbitMqPublisher publisher, ILogger<StudentAssignedToAssistantHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }
        public async Task Handle(StudentAssignedToAssistantDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Handling StudentAssignedToAssistantDomainEvent for Student {StudentId}", domainEvent.StudentId);

            var integrationEvent = new
            {
                StudentId = domainEvent.StudentId,
                AssistantId = domainEvent.AssistantId,
                OccurredOn = domainEvent.OccurredOn
            };

            string payload = JsonSerializer.Serialize(integrationEvent);

            await _publisher.PublishAsync(
                routingKey: "user.student.assigned",
                messageType: nameof(StudentAssignedToAssistantDomainEvent),
                messageBody: payload,
                cancellationToken: cancellationToken
            );
        }
    }
}
