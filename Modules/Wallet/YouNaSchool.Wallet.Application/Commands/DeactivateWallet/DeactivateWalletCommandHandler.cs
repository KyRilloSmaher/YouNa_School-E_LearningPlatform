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

namespace YouNaSchool.Wallet.Application.Commands.DeactivateWallet
{
    public class DeactivateWalletCommandHandler : IRequestHandler<DeactivateWalletCommand, Result<bool>>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeactivateWalletCommandHandler> _logger;

        public DeactivateWalletCommandHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork, ILogger<DeactivateWalletCommandHandler> logger)
        {
            _walletRepository = walletRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeactivateWalletCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var wallet = await _walletRepository.GetByIdAsync(request.WalletId);
                if (wallet == null)
                {
                    _logger.LogWarning("Wallet with ID {WalletId} not found.", request.WalletId);
                    return Result<bool>.Failure("Wallet not found", System.Net.HttpStatusCode.NotFound);
                }
                wallet.Deactivate();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Wallet with ID {WalletId} deactivated successfully.", request.WalletId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating wallet with ID {WalletId}", request.WalletId);
                return Result<bool>.Failure("An error occurred while deactivating the wallet", System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
