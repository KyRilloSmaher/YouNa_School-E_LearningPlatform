using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Infrastructure.Persistence;
using YouNaSchool.Wallet.Application.Abstractions.Persistence;
using YouNaSchool.Wallet.Domain.Entities;

namespace YouNaSchool.Wallet.Infrastructure.Persistence.Repositories
{
    class WalletLedgerEntryyRepository : Repository<WalletLedgerEntry> , IWalletLedgerEntryRepository
    {
        private readonly WalletDbContext _context;
        public WalletLedgerEntryyRepository(WalletDbContext context) : base(context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<WalletLedgerEntry>> GetByWalletIdAsync(Guid walletId)
        {
            return await _context.WalletLedgerEntries.Where(e => e.WalletId == walletId).ToListAsync();
        }
    }
}
