using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Application.DTOs
{
    public class WalletLedgerEntryResponseDTO
    {
        public Guid EntryId { get; init; }
        public Guid WalletId { get; init; }

        public decimal Amount { get; init; }
        public string Currency { get; init; } = null!;

        public string TransactionType { get; init; } = null!;
        public string Source { get; init; } = null!;

        public Guid ReferenceId { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
