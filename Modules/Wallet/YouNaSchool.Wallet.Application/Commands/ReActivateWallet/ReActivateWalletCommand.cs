using MediatR;
using Shared.Application.RESULT_PATTERN;

namespace YouNaSchool.Wallet.Application.Commands.ReActivateWallet
{
    public record ReActivateWalletCommand( Guid WalletId) : IRequest<Result<bool>>;
}
