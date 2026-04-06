using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace YouNaSchool.Wallet.Domain.Events
{
    public sealed class WalletDeactivatedEvent : DomainEvent
    {
        public Guid WalletId { get; }
        public string StudentId { get; }


        public WalletDeactivatedEvent(Guid walletId, string studentId)
        {
            WalletId = walletId;
            StudentId = studentId;
        }
    }
}
