using MediatR;
using Shared.Application.RESULT_PATTERN;
using YouNaSchool.Wallet.Application.DTOs;

namespace YouNaSchool.Wallet.Application.Queries.GetWalletLedgerEntry
{
    public record GetWalletLedgerEntryQuery (string StudentId) : IRequest<Result<IEnumerable<WalletLedgerEntryResponseDTO>>>;
}
