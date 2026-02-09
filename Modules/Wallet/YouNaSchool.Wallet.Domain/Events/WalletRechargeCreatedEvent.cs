using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Domain.Events
{
    public sealed class WalletRechargeCreatedEvent : DomainEvent
    {
        public Guid RechargeId { get; }
        public Guid WalletId { get; }
        public decimal Amount { get; }

        public WalletRechargeCreatedEvent(Guid rechargeId, Guid walletId, decimal amount)
        {
            RechargeId = rechargeId;
            WalletId = walletId;
            Amount = amount;
        }
    }
}
