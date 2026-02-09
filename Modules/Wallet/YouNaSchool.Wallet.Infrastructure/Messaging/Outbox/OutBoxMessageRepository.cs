using Microsoft.EntityFrameworkCore;
using SharedKernel.Application.OUTBOX_PATTERN;
using Wallet.Infrastructure.Persistence;

namespace YouNaSchool.Wallet.Infrastructure.Messaging.Outbox
{
    internal sealed class OutBoxMessageRepository : IOutBoxMessageRepository
    {
        private readonly WalletDbContext _context;

        public OutBoxMessageRepository(WalletDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OutboxMessage message,CancellationToken cancellationToken = default)
        {
            await _context.OutboxMessages.AddAsync(message, cancellationToken);
        }

        public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(int batchSize,CancellationToken cancellationToken = default)
        {
            return await _context.OutboxMessages
                                    .Where(m => m.ProcessedOn == null)
                                    .OrderBy(m => m.OccurredOn)
                                    .Take(batchSize)
                                    .ToListAsync(cancellationToken);
        }

        public async Task MarkAsProcessedAsync(Guid messageId,CancellationToken cancellationToken = default)
        {
            var message = await _context.OutboxMessages
                .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

            if (message is null)
                return;

            message.ProcessedOn = DateTime.UtcNow;
            message.Error = null;
        }

        public async Task MarkAsFailedAsync(Guid messageId , string error , CancellationToken cancellationToken = default)
        {
            var message = await _context.OutboxMessages
                .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

            if (message is null)
                return;

            message.Error = error;
        }
    }
}