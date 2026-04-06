

namespace SharedKernel.Application.Messaging.Outbox
{
    /// <summary>
    /// Repository for outbox messages — should be implemented per module using that module's DbContext
    /// </summary>
    public interface IOutBoxMessageRepository
    {
        Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(int batchSize = 50, CancellationToken cancellationToken = default);
        Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    }
}
