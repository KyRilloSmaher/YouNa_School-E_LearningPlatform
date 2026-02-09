using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Domain.Events
{
    public sealed class WalletCreditedEvent : DomainEvent
    {
        public Guid WalletId { get; }
        public decimal Amount { get; }

        public WalletCreditedEvent(Guid walletId, decimal amount)
        {
            WalletId = walletId;
            Amount = amount;
        }
    }
}
