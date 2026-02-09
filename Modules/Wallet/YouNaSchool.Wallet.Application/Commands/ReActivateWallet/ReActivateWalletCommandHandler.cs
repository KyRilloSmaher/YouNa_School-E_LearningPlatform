using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.UNIT_OF_WORK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Domain.Repositories;

namespace YouNaSchool.Wallet.Application.Commands.ReActivateWallet
{
    public class ReActivateWalletCommandHandler : IRequestHandler<ReActivateWalletCommand, Result<bool>>
    {
        public readonly IWalletRepository _walletRepository;
        private readonly ILogger<ReActivateWalletCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public ReActivateWalletCommandHandler(IWalletRepository walletRepository, ILogger<ReActivateWalletCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _walletRepository = walletRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(ReActivateWalletCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var wallet = await _walletRepository.GetByIdAsync(request.WalletId);
                if (wallet == null)
                {
                    _logger.LogWarning("Wallet with ID {WalletId} not found.", request.WalletId);
                    return Result<bool>.Failure("Wallet not found", System.Net.HttpStatusCode.NotFound);
                }
                if (wallet.IsActive)
                {
                    _logger.LogInformation("Wallet with ID {WalletId} is already active.", request.WalletId);
                    return Result<bool>.Success(true);
                }
                wallet.Reactivate();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Wallet with ID {WalletId} Reactivated successfully.", request.WalletId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Reactivating wallet with ID {WalletId}", request.WalletId);
                return Result<bool>.Failure("An error occurred while Reactivating the wallet", System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
