using MediatR;
using SharedKernel.Application.ClockandUserContext;
using SharedKernel.Application.OUTBOX_PATTERN;
using SharedKernel.Application.UNIT_OF_WORK;
using SharedKernel.Domain.CoreAbstractions;
using SharedKernel.Domain.Events;
using System.Text.Json;
using YounaSchool.Authuntication.Infrastructure.Persistence;

namespace YounaSchool.Authuntication.Infrastructure.Persistence;

/// <summary>
/// Unit of Work for the Authentication module.
/// Handles transaction management, domain events, outbox pattern, and MediatR publishing.
/// </summary>
internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _context;
    private readonly ISystemClock _clock;
    private readonly IMediator _mediator;

    public UnitOfWork(AuthDbContext context, ISystemClock clock, IMediator mediator)
    {
        _context = context;
        _clock = clock;
        _mediator = mediator;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var domainEvents = CollectDomainEvents();
            var outboxMessages = domainEvents
                .Select(CreateOutboxMessage)
                .ToList();

            if (outboxMessages.Count > 0)
                _context.Set<OutboxMessage>().AddRange(outboxMessages);

            var result = await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            ClearDomainEvents();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
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

    private static OutboxMessage CreateOutboxMessage(IDomainEvent domainEvent)
    {
        return  OutboxMessage.Create(
            type: domainEvent.GetType().Name,
            payload: JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
            exchange: "auth.events",
            routingKey:"auth"
        );
    }

    private void ClearDomainEvents()
    {
        foreach (var entry in _context.ChangeTracker.Entries<AggregateRoot>())
            entry.Entity.ClearDomainEvents();
    }
}
