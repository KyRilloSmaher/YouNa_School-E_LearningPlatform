using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Domain.Events
{
    public sealed class WalletCreatedEvent : DomainEvent
    {
        public Guid WalletId { get; }
        public string StudentId { get; }

        public WalletCreatedEvent(Guid walletId, string studentId)
        {
            WalletId = walletId;
            StudentId = studentId;
        }
    }
}
