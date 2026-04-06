using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Application.IntegrationEvents
{
    public interface IIntegrationEvent
    {
        DateTime OccurredOnUtc { get; }
        Guid EventId { get; }
    }
    /// <summary>
    /// Base class for integration events
    /// </summary>
    public abstract record IntegrationEvent : IIntegrationEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
    }
}
