using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedKernel.Application.UNIT_OF_WORK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Application.Abstractions.Messaging;
using YouNaSchool.Wallet.Application.Abstractions.Persistence;
using YouNaSchool.Wallet.Application.IntegrationEvents;
using YouNaSchool.Wallet.Domain.Entities;
using YouNaSchool.Wallet.Domain.Enums;
using YouNaSchool.Wallet.Domain.Repositories;

namespace YouNaSchool.Wallet.Infrastructure.IntegrationEvents.Handlers
{
    public class PaymentCompletedIntegrationEventHandler : IIntegrationEventHandler
    {
        private IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StudentRegistrationIntegrationEventHandler> _logger;
        private  IWalletRechargeRepository  _walletRechargeRepository;
        private  IWalletLedgerEntryRepository _walletLedgerEntryRepository;
        public PaymentCompletedIntegrationEventHandler( ILogger<StudentRegistrationIntegrationEventHandler> logger,IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task HandleAsync(string messageType, ReadOnlyMemory<byte> body, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _walletLedgerEntryRepository = scope.ServiceProvider.GetRequiredService<IWalletLedgerEntryRepository>();
            _walletRechargeRepository = scope.ServiceProvider.GetRequiredService<IWalletRechargeRepository>();
            if (messageType != "PaymentCompleted")
                return;
            var @event = JsonSerializer.Deserialize<PaymentCompletedIntegrationEvent>(body.Span);

            if (@event is null)
            {
                _logger.LogWarning("Received invalid Payment Completed event");
                return;
            }

            _logger.LogInformation("Received PaymentCompleted event");
            // Idempotency check
            var existingPayment = await _walletRechargeRepository.GetByIdAsync(@event.PaymentId, false, cancellationToken);

            if (existingPayment is null)
            { 
                _logger.LogInformation($" Trying To Handle Completed Payment Event on A Non Exisitng Payment With Id : {@event.PaymentId}");
                return;
            }
            if (existingPayment.Status != RechargeStatus.Pending)
            {
                _logger.LogInformation($" Trying To  Complete Payment Event on A Completed Exisitng Payment With Id : {@event.PaymentId}");
                return;
            }
            existingPayment.MarkCompleted(@event.PaymentIntentId);

            WalletLedgerEntry walletLedgerEntry =  WalletLedgerEntry.Credit(existingPayment.WalletId,existingPayment.Amount,WalletTransactionSource.Recharge,Guid.Parse(existingPayment.ProviderReferenceId));

            await _walletLedgerEntryRepository.AddAsync(walletLedgerEntry);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }
    }
}
