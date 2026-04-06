using SharedKernel.Domain.CoreAbstractions;
using YouNaSchool.Wallet.Domain.Entities;

namespace YouNaSchool.Wallet.Domain.Repositories
{

    /// <summary>
    /// Provides data access operations for <see cref="Wallets"/> entities.
    /// </summary>
    /// <remarks>
    /// This repository defines the contract for wallet data access operations.
    /// Implementations should provide persistence-agnostic functionality.
    /// </remarks>
    public interface IWalletRepository : IRepository<Wallets>
    {
        /// <inheritdoc cref="IRepository{Wallets}.GetByIdAsync(Guid, bool, CancellationToken)" />
        new Task<Wallets?> GetByIdAsync(Guid id, bool asTracked = true, CancellationToken cancellationToken = default);
        /// <inheritdoc cref="IRepository{Wallets}.AddAsync(Wallets, CancellationToken) />
        new Task AddAsync(Wallets wallet, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a wallet by the specified student identifier.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student.</param>
        /// <param name="asTracked">
        /// A value indicating whether the returned entity should be tracked by the context.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The wallet entity if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="studentId"/> is invalid.
        /// </exception>
        Task<Wallets?> GetByStudentIdAsync(string studentId, bool asTracked = false, CancellationToken cancellationToken = default);
    }
}