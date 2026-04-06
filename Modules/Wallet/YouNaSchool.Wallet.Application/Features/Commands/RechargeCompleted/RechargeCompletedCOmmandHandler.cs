using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.UNIT_OF_WORK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Domain.Enums;
using YouNaSchool.Wallet.Domain.Repositories;

namespace YouNaSchool.Wallet.Application.Features.Commands.RechargeCompleted
{
    public class RechargeCompletedCommandHandler : IRequestHandler<RechargeCompletedCommand, Result<bool>>
    {
        private readonly IWalletRechargeRepository _walletRechargeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RechargeCompletedCommandHandler> _logger;
        private readonly IWalletRepository _walletRepository;

        public RechargeCompletedCommandHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork, ILogger<RechargeCompletedCommandHandler> logger, IWalletRechargeRepository walletRechargeRepository)
        {
            _walletRechargeRepository = walletRechargeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _walletRepository = walletRepository;
        }

        public async Task<Result<bool>> Handle(RechargeCompletedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var recharge = await _walletRechargeRepository.GetByPaymentIntentIdAsync(request.ProviderReferenceId);
                if (recharge == null)
                {
                    _logger.LogWarning("Recharge not found for provider reference ID: {ProviderReferenceId}", request.ProviderReferenceId);
                    return Result<bool>.Failure("Recharge not found", System.Net.HttpStatusCode.NotFound);
                }
                recharge.MarkCompleted(request.ProviderReferenceId);
                var wallet = await _walletRepository.GetByIdAsync(recharge.WalletId , asTracked: true);
                if (wallet == null)
                {
                    _logger.LogWarning("Wallet not found for wallet ID: {WalletId}", recharge.WalletId);
                    return Result<bool>.Failure("Wallet not found", System.Net.HttpStatusCode.NotFound);
                }
                wallet.Credit(recharge.Amount, WalletTransactionSource.Recharge, recharge.Id);
                await _unitOfWork.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing recharge completion for provider reference ID: {ProviderReferenceId}", request.ProviderReferenceId);
                return Result<bool>.Failure("An error occurred while processing the recharge completion", System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
