using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Domain.Events
{
    public sealed class WalletDebitedEvent : DomainEvent
    {
        public Guid WalletId { get; }
        public decimal Amount { get; }
        public Guid ReferenceId { get; }

        public WalletDebitedEvent(Guid walletId, decimal amount, Guid referenceId)
        {
            WalletId = walletId;
            Amount = amount;
            ReferenceId = referenceId;
        }
    }
}
