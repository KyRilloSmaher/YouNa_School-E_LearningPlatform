using SharedKernel.Domain.CoreAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Models;

namespace YouNaSchool.Notifications.Domain.Interfaces.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Notification>> GetUnreadByUserAsync(Guid userId, CancellationToken ct = default);
        Task AddAsync(Notification notification, CancellationToken ct = default);
        Task UpdateAsync(Notification notification, CancellationToken ct = default);
    }
}
