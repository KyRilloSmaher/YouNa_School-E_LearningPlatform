using SharedKernel.Domain.CoreAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Domain.Entities;

namespace YouNaSchool.Wallet.Application.Abstractions.Persistence
{
    public interface IWalletLedgerEntryRepository : IRepository<WalletLedgerEntry>
    {
        Task<IEnumerable<WalletLedgerEntry>> GetByWalletIdAsync(Guid walletId);
    }
}
