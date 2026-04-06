using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Application.ClockandUserContext
{
    public interface ISystemClock
    {
        DateTime UtcNow { get; }
    }
}
