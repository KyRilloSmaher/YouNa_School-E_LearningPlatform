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
    public sealed class TeacherVerifiedHandler : INotificationHandler<TeacherVerifiedDomainEvent>
    {
        private readonly IRabbitMqPublisher _publisher;
        private readonly ILogger<TeacherVerifiedHandler> _logger;

        public TeacherVerifiedHandler(IRabbitMqPublisher publisher, ILogger<TeacherVerifiedHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(TeacherVerifiedDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Handling TeacherVerifiedDomainEvent for Teacher {TeacherId}", domainEvent.TeacherId);

            var integrationEvent = new
            {
                domainEvent.TeacherId,
                domainEvent.OccurredOn
            };

            string payload = JsonSerializer.Serialize(integrationEvent);

            await _publisher.PublishAsync(
                routingKey: "user.teacher.verified",
                messageType: nameof(TeacherVerifiedDomainEvent),
                messageBody: payload,
                cancellationToken: cancellationToken
            );
        }
    }
}
