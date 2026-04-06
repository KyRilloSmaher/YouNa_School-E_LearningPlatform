using SharedKernel.Domain.CoreAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Enums;
using YouNaSchool.Notifications.Domain.Models;

namespace YouNaSchool.Notifications.Domain.Interfaces.Repositories
{
    public interface INotificationPreferenceRepository : IRepository<NotificationPreference>
    {
        Task<bool> IsEnabledAsync(Guid userId,NotificationChannel channel,CancellationToken ct);
        Task<NotificationPreference?> GetByUserAndChannelAsync(Guid userId, NotificationChannel channel, CancellationToken ct = default);
        Task<List<NotificationPreference>> GetUserPreferencesAsync(Guid userId,CancellationToken ct);
        Task AddAsync(NotificationPreference preference, CancellationToken ct);
        Task UpdateAsync(NotificationPreference preference, CancellationToken ct);
       
    }
}
