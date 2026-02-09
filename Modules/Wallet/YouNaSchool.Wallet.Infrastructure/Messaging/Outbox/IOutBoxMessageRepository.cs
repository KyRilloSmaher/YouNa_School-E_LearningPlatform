using SharedKernel.Application.OUTBOX_PATTERN;

namespace YouNaSchool.Wallet.Infrastructure.Messaging.Outbox
{
    public interface IOutBoxMessageRepository
    {
        Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<OutboxMessage>>GetUnprocessedAsync(int batchSize, CancellationToken cancellationToken = default);

        Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);

        Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default);
    }
}
