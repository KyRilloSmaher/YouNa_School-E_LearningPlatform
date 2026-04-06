using Shared.Application.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Application.IntegrationEvent
{
    public interface IIntegrationEventPublisher
    {
        /// <summary>
        /// Saves the event to the outbox for reliable, transactional publishing.
        /// The event will be dispatched asynchronously after the current transaction commits.
        /// </summary>
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IIntegrationEvent;
    }
}
