using SharedKernel.Domain.CoreAbstractions;
using SharedKernel.Domain.EXCEPTIONS;
using SharedKernel.Domain.VALUE_OBJECTS;
using YouNaSchool.Wallet.Domain.Enums;
using YouNaSchool.Wallet.Domain.Events;

namespace YouNaSchool.Wallet.Domain.Entities
{
    /// <summary>
    /// Represents a wallet recharge transaction, including its status, amount, and associated payment provider
    /// information.
    /// </summary>
    /// <remarks>
    /// A <see cref="WalletRecharge"/> instance tracks the lifecycle of a wallet top-up operation, from initiation
    /// to completion or failure. It includes details such as the amount, payment provider, and relevant timestamps. The
    /// class is immutable except for properties intended for concurrency or client-side use. Instances are typically
    /// created when a user initiates a wallet recharge and are updated as the payment process progresses.
    /// </remarks>
    public sealed class WalletRecharge : Entity
    {
        /// <summary>
        /// Gets the unique identifier for this recharge transaction.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the unique identifier of the wallet being recharged.
        /// </summary>
        public Guid WalletId { get; private set; }


        /// <summary>
        /// Gets the unique identifier for the associated payment intent.
        /// </summary>
        public string ProviderReferenceId { get; private set; } = default!;

        /// <summary>
        /// Gets the monetary amount of the recharge.
        /// </summary>
        public Money Amount { get; private set; }

        /// <summary>
        /// Gets the current status of the recharge transaction.
        /// </summary>
        public RechargeStatus Status { get; private set; }

        /// <summary>
        /// Gets the name of the payment provider associated with the transaction.
        /// </summary>
        public PaymentProviders PaymentProvider { get; private set; }

        /// <summary>
        /// Gets the date and time when the recharge was initiated.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the recharge was completed or failed.
        /// </summary>
        /// <remarks>
        /// This property is null until the recharge reaches a terminal state (completed or failed).
        /// </remarks>
        public DateTime? CompletedAt { get; private set; }

        /// <summary>
        /// Gets or sets the client secret used for frontend payment confirmation.
        /// </summary>
        /// <remarks>
        /// This value is typically provided by the payment gateway and used by the client-side
        /// application to confirm payment intent with the payment provider.
        /// </remarks>
        public string? ClientPaymentToken { get; private set; }

        /// <summary>
        /// Gets or sets the version number for concurrency control and payment update tracking.
        /// </summary>
        /// <remarks>
        /// This property helps prevent race conditions when updating recharge status and
        /// ensures proper sequencing of payment provider callbacks.
        /// </remarks>
        public int Version { get; set; }

        /// <summary>
        /// Private constructor for Entity Framework Core materialization.
        /// </summary>
        private WalletRecharge() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletRecharge"/> class.
        /// </summary>
        /// <param name="walletId">The unique identifier of the wallet to be recharged.</param>
        /// <param name="amount">The monetary amount to recharge.</param>
        /// <param name="provider">The name of the payment provider handling the transaction.</param>
        /// <param name="providerReferenceId">The unique identifier for the payment intent from the payment provider.</param>
        /// <param name="clientPaymentToken">The client secret for frontend payment confirmation.</param>
        /// <exception cref="BusinessRuleViolationException">
        /// Thrown when the <paramref name="provider"/> is null, empty, or contains only whitespace.
        /// </exception>
        /// <remarks>
        /// Creates a new recharge transaction with <see cref="RechargeStatus.Pending"/> status,
        /// sets the initial version to 1, and raises a <see cref="WalletRechargeCreatedEvent"/> domain event.
        /// </remarks>
        public WalletRecharge(Guid walletId,Money amount, PaymentProviders provider,string providerReferenceId,string? clientPaymentToken)
        {
            Id = Guid.NewGuid();
            WalletId = walletId;

            Amount = amount;
            PaymentProvider = provider;
            ProviderReferenceId = providerReferenceId;
            ClientPaymentToken = clientPaymentToken;

            Status = RechargeStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            Version = 1;

            RaiseDomainEvent(
                new WalletRechargeCreatedEvent(Id, WalletId, amount.Amount)
            );
        }

        /// <summary>
        /// Marks the recharge transaction as completed.
        /// </summary>
        /// <exception cref="BusinessRuleViolationException">
        /// Thrown when the recharge is not in <see cref="RechargeStatus.Pending"/> status.
        /// </exception>
        /// <remarks>
        /// This method transitions the recharge status to <see cref="RechargeStatus.Completed"/>,
        /// sets the completion timestamp, and raises a <see cref="WalletRechargeCompletedEvent"/> domain event.
        /// </remarks>
       public void MarkCompleted(string providerReferenceId)
        {
            if (Status != RechargeStatus.Pending)
                throw new BusinessRuleViolationException(
                    "Only pending recharges can be completed."
                );

            if (ProviderReferenceId != providerReferenceId)
                throw new BusinessRuleViolationException(
                    "Provider reference mismatch."
                );

            Status = RechargeStatus.Completed;
            CompletedAt = DateTime.UtcNow;

            RaiseDomainEvent(
                new WalletRechargeCompletedEvent(Id, WalletId)
            );
        }

        /// <summary>
        /// Marks the recharge transaction as failed.
        /// </summary>
        /// <exception cref="BusinessRuleViolationException">
        /// Thrown when the recharge is not in <see cref="RechargeStatus.Pending"/> status.
        /// </exception>
        /// <remarks>
        /// This method transitions the recharge status to <see cref="RechargeStatus.Failed"/>,
        /// sets the failure timestamp, and raises a <see cref="WalletRechargeFailedEvent"/> domain event.
        /// </remarks>
        public void MarkFailed()
        {
            if (Status != RechargeStatus.Pending)
                throw new BusinessRuleViolationException(
                    "Only pending recharges can be failed."
                );

            Status = RechargeStatus.Failed;
            CompletedAt = DateTime.UtcNow;

            RaiseDomainEvent(
                new WalletRechargeFailedEvent(Id, WalletId)
            );
        }

        public void AttachProviderSession(string providerReferenceId, string clientPaymentToken)
        {
            if (string.IsNullOrWhiteSpace(providerReferenceId))
                throw new BusinessRuleViolationException("Provider reference ID cannot be empty.");

            if (string.IsNullOrWhiteSpace(clientPaymentToken))
                throw new BusinessRuleViolationException("Client payment token cannot be empty.");

            ProviderReferenceId = providerReferenceId;
            ClientPaymentToken = clientPaymentToken;
            Version++;
        }
    }
}