using Shared.Application.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Application.IntegrationEvents
{
    /// <summary>
    /// Published when a wallet transaction is completed
    /// </summary>
    public sealed record WalletTransactionCompletedIntegrationEvent(
        Guid TransactionId,
        Guid WalletId,
        Guid UserId,
        decimal Amount,
        string Currency,
        string TransactionType, // "deposit" | "withdrawal" | "transfer"
        DateTime CompletedAt) : IIntegrationEvent
    {
        public DateTime OccurredOnUtc => DateTime.UtcNow;

        public Guid EventId => Guid.NewGuid();
    }
}
