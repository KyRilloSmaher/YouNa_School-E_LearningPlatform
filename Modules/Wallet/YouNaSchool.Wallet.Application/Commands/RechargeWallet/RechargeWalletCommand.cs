using MediatR;
using Shared.Application.RESULT_PATTERN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Application.DTOs;
using YouNaSchool.Wallet.Domain.Enums;

namespace YouNaSchool.Wallet.Application.Commands.RechargeWallet
{
    public class RechargeWalletCommand:IRequest<Result<RechargeWalletResult>>
    {
        public Guid WalletId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; } = "EGP";
        public PaymentProviders Provider { get; init; }
        public string CallbackUrl { get; init; } = default!;
    }
}
