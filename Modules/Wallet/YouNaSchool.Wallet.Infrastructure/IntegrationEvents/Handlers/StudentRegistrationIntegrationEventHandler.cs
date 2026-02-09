using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedKernel.Application.UNIT_OF_WORK;
using System.Text.Json;
using YouNaSchool.Wallet.Application.Abstractions.Messaging;
using YouNaSchool.Wallet.Application.IntegrationEvents;
using YouNaSchool.Wallet.Domain.Entities;
using YouNaSchool.Wallet.Domain.Repositories;

namespace YouNaSchool.Wallet.Infrastructure.IntegrationEvents.Handlers
{
    public sealed class StudentRegistrationIntegrationEventHandler : IIntegrationEventHandler
    {
        private IWalletRepository _walletRepository;
        private IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StudentRegistrationIntegrationEventHandler> _logger;

        public StudentRegistrationIntegrationEventHandler(ILogger<StudentRegistrationIntegrationEventHandler> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task HandleAsync(string messageType,ReadOnlyMemory<byte> body,CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _walletRepository = scope.ServiceProvider.GetRequiredService<IWalletRepository>();

            if (messageType != "UserRegistered")
                return;

            var @event = JsonSerializer.Deserialize<StudentRegisteredIntegrationEvent>(body.Span);

            if (@event is null)
            {
                _logger.LogWarning("Received invalid UserRegistered event");
                return;
            }

            _logger.LogInformation(
                "Handling UserRegistered event for UserId {UserId}",
                @event.StudentId);

            // Idempotency check
            var existingWallet = await _walletRepository.GetByStudentIdAsync(@event.StudentId,false,cancellationToken);

            if (existingWallet is not null)
            {
                _logger.LogInformation(
                    "Wallet already exists for UserId {UserId}",
                    @event.StudentId);
                return;
            }

            var wallet = Wallets.Create(@event.StudentId);

            await  _walletRepository.AddAsync(wallet);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Wallet created for UserId {UserId}", @event.StudentId);
        }
    }
}