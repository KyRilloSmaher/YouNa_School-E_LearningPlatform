using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Domain.Enums;

namespace YouNaSchool.Wallet.Application.Abstractions.ExternalService.Payment
{
    public sealed class PaymentWebhookResult
    {
        public bool IsValid { get; init; }

        public string ProviderReferenceId { get; init; } = default!;
        public PaymentProviders Provider { get; init; }

        public RechargeStatus Status { get; init; }

        public decimal? Amount { get; init; }
    }
}
