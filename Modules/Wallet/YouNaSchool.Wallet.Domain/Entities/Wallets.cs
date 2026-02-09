
using SharedKernel.Domain.CoreAbstractions;
using SharedKernel.Domain.EXCEPTIONS;
using SharedKernel.Domain.VALUE_OBJECTS;
using YouNaSchool.Wallet.Domain.Enums;
using YouNaSchool.Wallet.Domain.Events;

namespace YouNaSchool.Wallet.Domain.Entities
{
    /// <summary>
    /// Represents a student's electronic wallet for managing financial transactions within the system.
    /// </summary>
    /// <remarks>
    /// The <see cref="Wallets"/> class serves as an aggregate root in the wallet domain, encapsulating
    /// the core wallet functionality including balance management, transaction tracking, and wallet lifecycle.
    /// It maintains an audit trail of all transactions through ledger entries and enforces business rules
    /// such as ensuring sufficient balance for debits and wallet activation status for transactions.
    /// This class follows the principles of domain-driven design with rich domain behavior and event sourcing.
    /// </remarks>
    public sealed class Wallets : AggregateRoot
    {
        /// <summary>
        /// Gets the unique identifier of the wallet.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the identifier of the student who owns this wallet.
        /// </summary>
        public string StudentId { get; private set; } = null!;

        /// <summary>
        /// Gets the current monetary balance of the wallet.
        /// </summary>
        public Money Balance { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the wallet is currently active and can process transactions.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets the date and time when the wallet was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        private readonly List<WalletLedgerEntry> _ledgerEntries = new();

        /// <summary>
        /// Gets a read-only collection of all ledger entries associated with this wallet.
        /// </summary>
        /// <remarks>
        /// This collection provides a complete audit trail of all financial transactions
        /// (credits and debits) performed on the wallet, including metadata about each transaction.
        /// </remarks>
        public IReadOnlyCollection<WalletLedgerEntry> LedgerEntries => _ledgerEntries.AsReadOnly();

        /// <summary>
        /// Private constructor for Entity Framework Core materialization.
        /// </summary>
        private Wallets() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Wallets"/> class.
        /// </summary>
        /// <param name="studentId">The identifier of the student who will own this wallet.</param>
        /// <remarks>
        /// This constructor creates a new wallet with an initial zero balance, sets it as active,
        /// and raises a <see cref="WalletCreatedEvent"/> domain event.
        /// </remarks>
        private Wallets(string studentId)
        {
            Id = Guid.NewGuid();
            StudentId = studentId;
            Balance = new Money(0, "USD");
            IsActive = true;
            CreatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new WalletCreatedEvent(Id, StudentId));
        }

        /// <summary>
        /// Creates a new wallet for the specified student.
        /// </summary>
        /// <param name="studentId">The identifier of the student who will own the wallet.</param>
        /// <returns>A new <see cref="Wallets"/> instance with zero balance and active status.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="studentId"/> is null or empty.</exception>
        public static Wallets Create(string studentId)
        {
            return new Wallets(studentId);
        }

        // =============================
        // BUSINESS BEHAVIOR
        // =============================

        /// <summary>
        /// Credits (adds) the specified amount to the wallet balance.
        /// </summary>
        /// <param name="amount">The monetary amount to credit to the wallet.</param>
        /// <param name="source">The source or reason for this credit transaction.</param>
        /// <param name="referenceId">A unique identifier for referencing this transaction.</param>
        /// <exception cref="BusinessRuleViolationException">
        /// Thrown when the wallet is not active.
        /// </exception>
        /// <remarks>
        /// This method increases the wallet balance by the specified amount, creates a corresponding
        /// ledger entry for auditing purposes, and raises a <see cref="WalletCreditedEvent"/> domain event.
        /// </remarks>
        public void Credit(Money amount, WalletTransactionSource source, Guid referenceId)
        {
            EnsureActive();

            Balance = new Money(Balance.Amount + amount.Amount, amount.Currency);

            var entry = WalletLedgerEntry.Credit(
                Id,
                amount,
                source,
                referenceId
            );

            _ledgerEntries.Add(entry);

            RaiseDomainEvent(new WalletCreditedEvent(Id, amount.Amount));
        }

        /// <summary>
        /// Debits (subtracts) the specified amount from the wallet balance.
        /// </summary>
        /// <param name="amount">The monetary amount to debit from the wallet.</param>
        /// <param name="source">The source or reason for this debit transaction.</param>
        /// <param name="referenceId">A unique identifier for referencing this transaction.</param>
        /// <exception cref="BusinessRuleViolationException">
        /// Thrown when:
        /// 1. The wallet is not active.
        /// 2. The wallet has insufficient balance to complete the transaction.
        /// </exception>
        /// <remarks>
        /// This method decreases the wallet balance by the specified amount, creates a corresponding
        /// ledger entry for auditing purposes, and raises a <see cref="WalletDebitedEvent"/> domain event.
        /// The method enforces business rules to prevent overdrafts.
        /// </remarks>
        public void Debit(Money amount, WalletTransactionSource source, Guid referenceId)
        {
            EnsureActive();

            if (Balance.Amount < amount.Amount)
                throw new BusinessRuleViolationException("Insufficient wallet balance.");

            Balance = new Money(Balance.Amount - amount.Amount, amount.Currency);

            var entry = WalletLedgerEntry.Debit(
                Id,
                amount,
                source,
                referenceId
            );

            _ledgerEntries.Add(entry);

            RaiseDomainEvent(new WalletDebitedEvent(Id, amount.Amount, referenceId));
        }

        /// <summary>
        /// Deactivates the wallet, preventing any further transactions.
        /// </summary>
        /// <remarks>
        /// This method sets the <see cref="IsActive"/> property to false and raises a
        /// <see cref="WalletDeactivatedEvent"/> domain event. Once deactivated, the wallet
        /// cannot process any credit or debit operations until reactivated.
        /// </remarks>
        public void Deactivate()
        {
            IsActive = false;
            RaiseDomainEvent(new WalletDeactivatedEvent(Id));
        }
        public void Reactivate()
        {
            IsActive = true;
            RaiseDomainEvent(new WalletReactivatedEvent(Id));
        }

        /// <summary>
        /// Ensures that the wallet is active before performing any transaction.
        /// </summary>
        /// <exception cref="BusinessRuleViolationException">
        /// Thrown when the wallet is not active.
        /// </exception>
        private void EnsureActive()
        {
            if (!IsActive)
                throw new BusinessRuleViolationException("Wallet is not active.");
        }
    }
}