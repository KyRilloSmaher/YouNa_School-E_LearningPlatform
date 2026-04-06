using Microsoft.Extensions.Logging;
using SharedKernel.Application.UNIT_OF_WORK;
using YouNaSchhol.Users.Application.IntegrationEvents;
using YouNaSchool.Wallet.Domain.Entities;
using YouNaSchool.Wallet.Domain.Repositories;

namespace YouNaSchool.Wallet.Infrastructure.IntegrationEvents.Handlers;

internal sealed class StudentRegisteredIntegrationEventHandler : IIntegrationEventHandler<StudentCreatedIntegrationEvent>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StudentRegisteredIntegrationEventHandler> _logger;

    public StudentRegisteredIntegrationEventHandler(
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork,
        ILogger<StudentRegisteredIntegrationEventHandler> logger)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(
        StudentCreatedIntegrationEvent @event,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling StudentCreated event for StudentId {StudentId} ({Email})",
            @event.UserId,
            @event.Email);

        try
        {
            var existingWallet = await _walletRepository.GetByStudentIdAsync(
                @event.UserId.ToString(),
                asTracked: false,
                cancellationToken);

            if (existingWallet is not null)
            {
                _logger.LogInformation(
                    "Wallet already exists for StudentId {StudentId}. Skipping creation.",
                    @event.UserId);
                return;
            }

            var wallet = Wallets.Create(@event.UserId.ToString());

            await _walletRepository.AddAsync(wallet);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully created wallet for StudentId {StudentId}",
                @event.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to create wallet for StudentId {StudentId}",
                @event.UserId);
            throw;
        }
    }
}
