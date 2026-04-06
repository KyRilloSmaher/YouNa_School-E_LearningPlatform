using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Application.DTOs
{
    public class StudentWalletResponseDTO
    {
        public Guid WalletId { get; init; }
        public string StudentId { get; init; } = null!;

        public decimal Balance { get; init; }
        public string Currency { get; init; } = null!;

        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
