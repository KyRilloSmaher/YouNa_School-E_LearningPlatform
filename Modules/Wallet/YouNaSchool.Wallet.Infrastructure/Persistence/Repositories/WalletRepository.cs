using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Persistence;
using Wallet.Infrastructure.Persistence;
using YouNaSchool.Wallet.Domain.Entities;
using YouNaSchool.Wallet.Domain.Repositories;

namespace YouNaSchool.Wallet.Infrastructure.Repositories
{



    /// <summary>
    /// Entity Framework Core implementation of <see cref="IWalletRepository"/>.
    /// </summary>
    /// <remarks>
    /// This implementation uses EF Core for data persistence and includes
    /// wallet-specific query optimizations.
    /// </remarks>
    public sealed class WalletRepository : Repository<Wallets>, IWalletRepository
    {
        private readonly WalletDbContext _context;
        /// <summary>
        /// Initializes a new instance using the specified database context.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public WalletRepository(WalletDbContext context) : base(context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<Wallets?> GetByStudentIdAsync(string studentId, bool asTracked = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("Student ID cannot be null or empty.", nameof(studentId));

            // Base query that includes ledger entries
            var query = _context.Wallets
                .Include(w => w.LedgerEntries)
                .Where(w => w.StudentId == studentId);

            // Apply tracking preference
            if (!asTracked)
            {
                query = query.AsNoTracking();
            }

            // Execute the query and return the result
            return await query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}