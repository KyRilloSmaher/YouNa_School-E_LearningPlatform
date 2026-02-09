using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Domain.Events
{
    public sealed class WalletRechargeCompletedEvent : DomainEvent
    {
        public Guid RechargeId { get; }
        public Guid WalletId { get; }

        public WalletRechargeCompletedEvent(Guid rechargeId, Guid walletId)
        {
            RechargeId = rechargeId;
            WalletId = walletId;
        }
    }
}
