
using Shared.Application.IntegrationEvents;

/// <summary>
/// Handler interface for integration events
/// </summary>
/// <typeparam name="TEvent">The integration event type</typeparam>
public interface IIntegrationEventHandler<in TEvent>
    where TEvent : IIntegrationEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}