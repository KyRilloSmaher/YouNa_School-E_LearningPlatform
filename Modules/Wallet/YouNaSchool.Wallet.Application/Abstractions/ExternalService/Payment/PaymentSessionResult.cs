using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Domain.Enums;

namespace YouNaSchool.Wallet.Application.Abstractions.ExternalService.Payment
{
    public sealed class PaymentSessionResult
    {
        public string ProviderReferenceId { get; init; } = default!;
        public string? ClientPaymentToken { get; init; }
        public PaymentProviders Provider { get; init; }
    }
}
