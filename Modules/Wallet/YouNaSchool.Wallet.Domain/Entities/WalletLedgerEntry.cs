using SharedKernel.Domain.CoreAbstractions;
using SharedKernel.Domain.EXCEPTIONS;
using SharedKernel.Domain.VALUE_OBJECTS;
using YouNaSchool.Wallet.Domain.Enums;

namespace YouNaSchool.Wallet.Domain.Entities
{
    /// <summary>
    /// Represents an individual entry in the wallet's transaction ledger, providing an audit trail
    /// for all financial transactions.
    /// </summary>
    /// <remarks>
    /// The <see cref="WalletLedgerEntry"/> class serves as an immutable record of a single financial
    /// transaction within a wallet. Each entry captures essential details including the transaction
    /// amount, type (credit/debit), source, and reference identifiers. These entries collectively
    /// form a complete audit trail that supports financial reporting, dispute resolution, and
    /// transaction history analysis.
    /// </remarks>
    public sealed class WalletLedgerEntry : Entity
    {
        /// <summary>
        /// Gets the unique identifier of this ledger entry.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the unique identifier of the wallet associated with this ledger entry.
        /// </summary>
        public Guid WalletId { get; private set; }

        /// <summary>
        /// Gets the monetary amount of the transaction.
        /// </summary>
        public Money Amount { get; private set; }

        /// <summary>
        /// Gets the type of transaction (credit or debit).
        /// </summary>
        public WalletTransactionType TransactionType { get; private set; }

        /// <summary>
        /// Gets the source or reason for this transaction.
        /// </summary>
        public WalletTransactionSource Source { get; private set; }

        /// <summary>
        /// Gets the unique identifier referencing the original transaction or related entity.
        /// </summary>
        public Guid ReferenceId { get; private set; }

        /// <summary>
        /// Gets the date and time when this ledger entry was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Private constructor for Entity Framework Core materialization.
        /// </summary>
        private WalletLedgerEntry() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletLedgerEntry"/> class.
        /// </summary>
        /// <param name="walletId">The unique identifier of the wallet.</param>
        /// <param name="amount">The monetary amount of the transaction.</param>
        /// <param name="type">The type of transaction (credit or debit).</param>
        /// <param name="source">The source or reason for this transaction.</param>
        /// <param name="referenceId">A unique identifier for referencing this transaction.</param>
        /// <exception cref="BusinessRuleViolationException">
        /// Thrown when:
        /// 1. <paramref name="walletId"/> is an empty GUID.
        /// 2. <paramref name="referenceId"/> is an empty GUID.
        /// 3. <paramref name="amount"/> is less than or equal to zero.
        /// </exception>
        private WalletLedgerEntry(
            Guid walletId,
            Money amount,
            WalletTransactionType type,
            WalletTransactionSource source,
            Guid referenceId)
        {
            if (walletId == Guid.Empty)
                throw new BusinessRuleViolationException("WalletId is required.");

            if (referenceId == Guid.Empty)
                throw new BusinessRuleViolationException("ReferenceId is required.");

            if (amount.Amount <= 0)
                throw new BusinessRuleViolationException("Amount must be greater than zero.");

            Id = Guid.NewGuid();
            WalletId = walletId;
            Amount = amount;
            TransactionType = type;
            Source = source;
            ReferenceId = referenceId;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a new credit ledger entry.
        /// </summary>
        /// <param name="walletId">The unique identifier of the wallet receiving the credit.</param>
        /// <param name="amount">The monetary amount to credit.</param>
        /// <param name="source">The source or reason for this credit transaction.</param>
        /// <param name="referenceId">A unique identifier for referencing this transaction.</param>
        /// <returns>A new <see cref="WalletLedgerEntry"/> instance representing a credit transaction.</returns>
        /// <exception cref="BusinessRuleViolationException">
        /// Thrown when any of the input parameters violate business rules.
        /// </exception>
        public static WalletLedgerEntry Credit(
            Guid walletId,
            Money amount,
            WalletTransactionSource source,
            Guid referenceId)
        {
            return new WalletLedgerEntry(
                walletId,
                amount,
                WalletTransactionType.Credit,
                source,
                referenceId
            );
        }

        /// <summary>
        /// Creates a new debit ledger entry.
        /// </summary>
        /// <param name="walletId">The unique identifier of the wallet from which the debit is made.</param>
        /// <param name="amount">The monetary amount to debit.</param>
        /// <param name="source">The source or reason for this debit transaction.</param>
        /// <param name="referenceId">A unique identifier for referencing this transaction.</param>
        /// <returns>A new <see cref="WalletLedgerEntry"/> instance representing a debit transaction.</returns>
        /// <exception cref="BusinessRuleViolationException">
        /// Thrown when any of the input parameters violate business rules.
        /// </exception>
        public static WalletLedgerEntry Debit(
            Guid walletId,
            Money amount,
            WalletTransactionSource source,
            Guid referenceId)
        {
            return new WalletLedgerEntry(
                walletId,
                amount,
                WalletTransactionType.Debit,
                source,
                referenceId
            );
        }
    }
}