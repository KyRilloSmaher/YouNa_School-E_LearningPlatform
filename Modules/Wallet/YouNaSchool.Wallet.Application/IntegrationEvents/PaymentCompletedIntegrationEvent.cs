using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Application.IntegrationEvents
{
    public class PaymentCompletedIntegrationEvent
    {
        public Guid PaymentId { get; set; }
        public string PaymentIntentId { get; set; } = null!;
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CompletedAt { get; set; }
        public DateTime OccurredOn { get; init; }
    }
}
