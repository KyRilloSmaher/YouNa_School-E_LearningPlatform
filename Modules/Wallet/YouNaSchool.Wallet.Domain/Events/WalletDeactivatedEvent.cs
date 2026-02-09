using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Domain.Events
{
    public sealed class WalletDeactivatedEvent : DomainEvent
    {
        public Guid WalletId { get; }

        public WalletDeactivatedEvent(Guid walletId)
        {
            WalletId = walletId;
        }
    }
}
