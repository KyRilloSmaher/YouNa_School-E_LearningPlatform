using SharedKernel.Application.OUTBOX_PATTERN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchhol.Users.Application.Abstractions.persistence
{
    public interface IOutboxMessageRepository
    {
        Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(
            int batchSize,
            CancellationToken cancellationToken = default);

        Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);

        Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default);
    }
}
