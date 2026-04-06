

using MediatR;

namespace SharedKernel.Domain.Events
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }

}
