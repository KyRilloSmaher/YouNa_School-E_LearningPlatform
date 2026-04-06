using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Domain.Enums;

namespace YouNaSchool.Wallet.Application.Abstractions.ExternalService.Payment
{
    public sealed class CreatePaymentSessionCommand
    {
        public Guid WalletId { get; init; }
        public Guid RechargeId { get; init; }

        public decimal Amount { get; init; }
        public string Currency { get; init; } = "Egp";

        public PaymentProviders Provider { get; init; }

        public string CallbackUrl { get; init; } = default!;
    }
}
