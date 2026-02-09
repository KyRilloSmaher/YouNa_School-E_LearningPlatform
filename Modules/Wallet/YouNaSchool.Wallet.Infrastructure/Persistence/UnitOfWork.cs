
using Microsoft.EntityFrameworkCore;
using SharedKernel.Application.ClockandUserContext;
using SharedKernel.Application.OUTBOX_PATTERN;
using SharedKernel.Application.UNIT_OF_WORK;
using SharedKernel.Domain.CoreAbstractions;
using SharedKernel.Domain.Events;
using System.Text.Json;


namespace Wallet.Infrastructure.Persistence.UnitOfWork

{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly WalletDbContext _context;
        private readonly ISystemClock _clock;

        public UnitOfWork(WalletDbContext context, ISystemClock clock)
        {
            _context = context;
            _clock = clock;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            // 1 -> Begin transaction
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            try
            {

                // 2 -> Collect domain events
                var domainEvents = CollectDomainEvents();

                // 3-> Convert domain events to Outbox messages
                var outboxMessages = domainEvents
                                            .Select(CreateOutboxMessage)
                                            .ToList();

                if (outboxMessages.Any())
                    _context.Set<OutboxMessage>().AddRange(outboxMessages);

                // 4 -> Save all changes
                var result = await _context.SaveChangesAsync(ct);

                // 5 -> Commit transaction
                await transaction.CommitAsync(ct);

                // 6 ->  Clear domain events AFTER commit
                ClearDomainEvents();

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
        private List<IDomainEvent> CollectDomainEvents()
        {
            return _context.ChangeTracker
                .Entries<AggregateRoot>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();
        }
        private OutboxMessage CreateOutboxMessage(IDomainEvent domainEvent)
        {
            return new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = domainEvent.GetType().Name,
                Payload = JsonSerializer.Serialize(
                    domainEvent,
                    domainEvent.GetType()),
                OccurredOn = _clock.UtcNow
            };
        }
        private void ClearDomainEvents()
        {
            foreach (var entry in _context.ChangeTracker.Entries<AggregateRoot>())
                entry.Entity.ClearDomainEvents();
        }
    }
}