using SharedKernel.Domain.CoreAbstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Domain.Entities;

namespace YouNaSchool.Wallet.Domain.Repositories
{
    /// <summary>
    /// Defines the contract for wallet recharge-specific data access operations.
    /// </summary>
    /// <remarks>
    /// This repository extends the generic repository pattern with wallet recharge-specific
    /// queries and operations. It follows the domain-driven design principle of having
    /// domain-specific repositories for aggregate roots.
    /// </remarks>
    public interface IWalletRechargeRepository : IRepository<WalletRecharge>
    {
        /// <summary>
        /// Retrieves a wallet recharge by the payment intent identifier.
        /// </summary>
        /// <param name="paymentIntentId">The payment intent identifier from the payment provider.</param>
        /// <param name="asTracked">
        /// A value indicating whether the returned entity should be tracked by the context.
        /// Default is false since recharge status updates are typically handled separately.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The wallet recharge entity if found; otherwise, null.
        /// </returns>
        Task<WalletRecharge?> GetByPaymentIntentIdAsync(string paymentIntentId,bool asTracked = false,CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves pending recharges for a specific wallet.
        /// </summary>
        /// <param name="walletId">The unique identifier of the wallet.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A collection of pending recharge transactions for the specified wallet.
        /// </returns>
        Task<IEnumerable<WalletRecharge>> GetPendingRechargesByWalletIdAsync(Guid walletId,CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves wallet recharges within a specified date range.
        /// </summary>
        /// <param name="walletId">The unique identifier of the wallet.</param>
        /// <param name="startDate">The start date of the range (inclusive).</param>
        /// <param name="endDate">The end date of the range (inclusive).</param>
        /// <param name="asTracked">
        /// Whether to track the entities (default is false for reporting).
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A collection of recharge transactions within the specified date range.
        /// </returns>
        Task<IEnumerable<WalletRecharge>> GetRechargesByDateRangeAsync(Guid walletId,DateTime startDate,DateTime endDate,bool asTracked = false,CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a pending recharge exists for the specified wallet.
        /// </summary>
        /// <param name="walletId">The unique identifier of the wallet.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// True if a pending recharge exists; otherwise, false.
        /// </returns>
        /// <remarks>
        /// Useful for preventing duplicate recharge attempts or validating business rules.
        /// </remarks>
        Task<bool> HasPendingRechargeAsync(Guid walletId,CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the total recharge amount for a wallet within a specified period.
        /// </summary>
        /// <param name="walletId">The unique identifier of the wallet.</param>
        /// <param name="startDate">The start date of the period.</param>
        /// <param name="endDate">The end date of the period.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The total amount recharged in the specified period.
        /// </returns>
        Task<decimal> GetTotalRechargedAmountAsync( Guid walletId,DateTime startDate,DateTime endDate,CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the version of a wallet recharge for optimistic concurrency control.
        /// </summary>
        /// <param name="rechargeId">The unique identifier of the recharge.</param>
        /// <param name="expectedVersion">The expected current version.</param>
        /// <param name="newVersion">The new version to set.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// True if the version was updated successfully; false if the expected version
        /// doesn't match (indicating concurrent modification).
        /// </returns>
        /// <remarks>
        /// This method uses optimistic concurrency control to handle payment webhook
        /// callbacks that may arrive concurrently.
        /// </remarks>
        //Task<bool> TryUpdateVersionAsync(Guid rechargeId,int expectedVersion,int newVersion,CancellationToken cancellationToken = default);
    }
}