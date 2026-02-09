using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Persistence;
using Wallet.Infrastructure.Persistence;
using YouNaSchool.Wallet.Domain.Entities;
using YouNaSchool.Wallet.Domain.Enums;
using YouNaSchool.Wallet.Domain.Repositories;

namespace YouNaSchool.Wallet.Infrastructure.Repositories
{
    /// <summary>
    /// Entity Framework Core implementation of <see cref="IWalletRechargeRepository"/>.
    /// </summary>
    /// <remarks>
    /// This repository provides wallet recharge-specific data access operations using EF Core.
    /// It includes optimized queries for common recharge scenarios and handles the unique
    /// requirements of payment transaction tracking.
    /// </remarks>
    public sealed class WalletRechargeRepository : Repository<WalletRecharge>, IWalletRechargeRepository
    {
        private readonly WalletDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletRechargeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context for wallet data access.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="context"/> is null.
        /// </exception>
        public WalletRechargeRepository(WalletDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/>
        public async Task<WalletRecharge?> GetByPaymentIntentIdAsync(string paymentIntentId,bool asTracked = false,CancellationToken cancellationToken = default)
        {
            var query = _context.WalletRecharges.Where(wr => wr.ProviderReferenceId == paymentIntentId);
            return   asTracked ? await query.AsTracking().FirstOrDefaultAsync() : await query.AsNoTracking().FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<WalletRecharge>> GetPendingRechargesByWalletIdAsync(Guid walletId,CancellationToken cancellationToken = default)
        {
            return await FindAsync(
                predicate: r => r.WalletId == walletId && r.Status == RechargeStatus.Pending,
                asTracked: false,
                cancellationToken: cancellationToken
            );
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<WalletRecharge>> GetRechargesByDateRangeAsync(
            Guid walletId,
            DateTime startDate,
            DateTime endDate,
            bool asTracked = false,
            CancellationToken cancellationToken = default)
        {
            // Ensure dates are in UTC for consistent comparison
            var utcStartDate = startDate.ToUniversalTime();
            var utcEndDate = endDate.ToUniversalTime();

            var query = GetQuery(asTracked)
                .Where(r => r.WalletId == walletId)
                .Where(r => r.CreatedAt >= utcStartDate && r.CreatedAt <= utcEndDate)
                .OrderByDescending(r => r.CreatedAt); // Most recent first

            return await query.ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> HasPendingRechargeAsync(
            Guid walletId,
            CancellationToken cancellationToken = default)
        {
            return await GetQuery(asTracked: false)
                .AnyAsync(
                    r => r.WalletId == walletId && r.Status == RechargeStatus.Pending,
                    cancellationToken
                );
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalRechargedAmountAsync(
            Guid walletId,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default)
        {
            var utcStartDate = startDate.ToUniversalTime();
            var utcEndDate = endDate.ToUniversalTime();

            var total = await GetQuery(asTracked: false)
                .Where(r => r.WalletId == walletId)
                .Where(r => r.Status == RechargeStatus.Completed) // Only count successful recharges
                .Where(r => r.CompletedAt >= utcStartDate && r.CompletedAt <= utcEndDate)
                .SumAsync(r => r.Amount.Amount, cancellationToken);

            return total;
        }

        ///// <inheritdoc/>
        //public async Task<bool> TryUpdateVersionAsync(
        //    Guid rechargeId,
        //    int expectedVersion,
        //    int newVersion,
        //    CancellationToken cancellationToken = default)
        //{
        //    // Use raw SQL for atomic version update to handle concurrent webhooks
        //    var sql = @"
        //        UPDATE WalletRecharges 
        //        SET Version = @newVersion 
        //        WHERE Id = @rechargeId AND Version = @expectedVersion";

        //    var parameters = new[]
        //    {
        //        new Microsoft.Data.SqlClient.SqlParameter("@rechargeId", rechargeId),
        //        new Microsoft.Data.SqlClient.SqlParameter("@expectedVersion", expectedVersion),
        //        new Microsoft.Data.SqlClient.SqlParameter("@newVersion", newVersion)
        //    };

        //    var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
        //        sql,
        //        parameters,
        //        cancellationToken
        //    );

        //    return rowsAffected > 0;
        //}

        /// <summary>
        /// Gets a queryable source for wallet recharges with common includes.
        /// </summary>
        /// <param name="asTracked">Whether to track entities.</param>
        /// <returns>A queryable source for wallet recharges.</returns>
        /// <remarks>
        /// This method can be extended to include commonly needed related entities.
        /// </remarks>
        private IQueryable<WalletRecharge> GetWalletRechargeQuery(bool asTracked = false)
        {
            // Start with the base query
            return GetQuery(asTracked)
                // Add any common ordering
                .OrderByDescending(r => r.CreatedAt);
        }

        /// <summary>
        /// Retrieves recharges by status for administrative reporting.
        /// </summary>
        /// <param name="status">The recharge status to filter by.</param>
        /// <param name="pageNumber">The page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paged list of recharges with the specified status.</returns>
        /// <remarks>
        /// This method demonstrates how to add domain-specific query methods
        /// that aren't part of the interface but are useful for the implementation.
        /// </remarks>
        public async Task<(IEnumerable<WalletRecharge> Items, int TotalCount)> GetRechargesByStatusPagedAsync(
            RechargeStatus status,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = GetWalletRechargeQuery(asTracked: false)
                .Where(r => r.Status == status);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}