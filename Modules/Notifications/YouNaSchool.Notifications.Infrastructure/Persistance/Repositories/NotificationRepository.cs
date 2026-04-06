using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Persistence;
using YouNaSchool.Notifications.Domain.Interfaces.Repositories;
using YouNaSchool.Notifications.Domain.Models;
using YouNaSchool.Notifications.Infrastructure.Persistance;

namespace YouNaSchool.Notifications.Infrastructure.Repositories
{
    public class NotificationRepository :Repository<Notification> , INotificationRepository
    {
        private readonly NotificationDbContext _dbContext;

        public NotificationRepository(NotificationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.Notifications
                .FirstOrDefaultAsync(n => n.Id == id, ct);
        }

        public async Task<IEnumerable<Notification>> GetUnreadByUserAsync(Guid userId, CancellationToken ct = default)
        {
            return await _dbContext.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Notification notification, CancellationToken ct = default)
        {
            await _dbContext.Notifications.AddAsync(notification, ct);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Notification notification, CancellationToken ct = default)
        {
            _dbContext.Notifications.Update(notification);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}