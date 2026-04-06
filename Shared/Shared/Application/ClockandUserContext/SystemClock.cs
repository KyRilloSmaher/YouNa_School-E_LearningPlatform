

using SharedKernel.Application.ClockandUserContext;

namespace Shared.Application.ClockandUserContext
{
    public sealed class SystemClock : ISystemClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
