using MediatR;
using Shared.Application.RESULT_PATTERN;

namespace YouNaSchool.Wallet.Application.Features.Commands.ReActivateWallet
{
    public record ReActivateWalletCommand( Guid WalletId) : IRequest<Result<bool>>;
}
