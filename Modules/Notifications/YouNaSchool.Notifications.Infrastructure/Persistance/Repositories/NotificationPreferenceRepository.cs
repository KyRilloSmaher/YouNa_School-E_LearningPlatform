using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.CoreAbstractions;
using SharedKernel.Infrastructure.Persistence;
using YouNaSchool.Notifications.Domain.Enums;
using YouNaSchool.Notifications.Domain.Interfaces.Repositories;
using YouNaSchool.Notifications.Domain.Models;
using YouNaSchool.Notifications.Infrastructure.Persistance;

namespace YouNaSchool.Notifications.Infrastructure.Repositories
{
    public class NotificationPreferenceRepository :Repository<NotificationPreference> , INotificationPreferenceRepository
    {
        private readonly NotificationDbContext _dbContext;

        public NotificationPreferenceRepository(NotificationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsEnabledAsync(Guid userId, NotificationChannel channel, CancellationToken ct)
        {
            var pref = await _dbContext.NotificationPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(np => np.UserId == userId && np.Channel == channel, ct);

            return pref?.IsEnabled ?? false;
        }

        public async Task<NotificationPreference?> GetByUserAndChannelAsync(Guid userId, NotificationChannel channel, CancellationToken ct = default)
        {
            return await _dbContext.NotificationPreferences
                .FirstOrDefaultAsync(np => np.UserId == userId && np.Channel == channel, ct);
        }

        public async Task<List<NotificationPreference>> GetUserPreferencesAsync(Guid userId, CancellationToken ct)
        {
            return await _dbContext.NotificationPreferences
                .Where(np => np.UserId == userId)
                .ToListAsync(ct);
        }

        public async Task AddAsync(NotificationPreference preference, CancellationToken ct)
        {
            await _dbContext.NotificationPreferences.AddAsync(preference, ct);

        }

        public async Task UpdateAsync(NotificationPreference preference, CancellationToken ct)
        {
            _dbContext.NotificationPreferences.Update(preference);

        }
    }
}