using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Application.IntegrationEvents
{
    public sealed class StudentRegisteredIntegrationEvent
    {
        public string StudentId { get; init; }
        public string Email { get; init; } = null!;
        public DateTime OccurredOn { get; init; }
    }
}
