using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Notifications.Domain.Interfaces.Services
{
    public interface ISmsService
    {
        Task SendSmsAsync(string phone, string message);
    }
}
