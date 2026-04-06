using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Application.DTOs
{
    public sealed class RechargeWalletResult
    {
        public Guid RechargeId { get; init; }
        public string ProviderReferenceId { get; init; } = default!;
        public string? ClientPaymentToken { get; init; }
    }
}
