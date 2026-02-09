using MediatR;
using Shared.Application.RESULT_PATTERN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Application.Commands.DeactivateWallet
{
    public class DeactivateWalletCommand : IRequest<Result<bool>>
    {
        public Guid WalletId { get; set; }
        public DeactivateWalletCommand(Guid walletId)
        {
            WalletId = walletId;
        }
    }
}
