using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Notifications.Domain.Interfaces.Services
{
    public interface IPushService
    {
        Task SendPushAsync(Guid userId, string message);
    }
}
