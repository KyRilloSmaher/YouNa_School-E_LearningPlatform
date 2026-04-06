using Microsoft.EntityFrameworkCore;
using SharedKernel.Application.Messaging.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YounaSchool.Authuntication.Infrastructure.Persistence.Repositories
{
    public class OutBoxRepository : IOutBoxMessageRepository
    {
        private readonly AuthDbContext _dbContext;

        public OutBoxRepository(AuthDbContext dbContext)
            => _dbContext = dbContext;

        public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
            => await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);

        public async Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(
            int batchSize = 50,
            CancellationToken cancellationToken = default)
            => await _dbContext.OutboxMessages
                .Where(m => m.PublishedAt == null && m.RetryCount < 5)
                .OrderBy(m => m.CreatedAt)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

        public async Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            _dbContext.OutboxMessages.Update(message);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
