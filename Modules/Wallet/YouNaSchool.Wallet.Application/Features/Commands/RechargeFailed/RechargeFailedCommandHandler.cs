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

namespace YouNaSchool.Wallet.Application.Features.Commands.RechargeFailed
{
    public class RechargeFailedCommandHandler : IRequestHandler<RechargeFailedCommand, Result<bool>>
    {
        private readonly IWalletRechargeRepository _walletRechargeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RechargeFailedCommandHandler> _logger;

        public RechargeFailedCommandHandler(IUnitOfWork unitOfWork, ILogger<RechargeFailedCommandHandler> logger, IWalletRechargeRepository walletRechargeRepository)
        {
            _walletRechargeRepository = walletRechargeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(RechargeFailedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var recharge = await _walletRechargeRepository.GetByPaymentIntentIdAsync(request.ProviderReferenceId);
                if (recharge == null)
                {
                    _logger.LogWarning("Recharge not found for provider reference ID: {ProviderReferenceId}", request.ProviderReferenceId);
                    return Result<bool>.Failure("Recharge not found", System.Net.HttpStatusCode.NotFound);
                }
                recharge.MarkFailed();
                await _unitOfWork.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing recharge failure for provider reference ID: {ProviderReferenceId}", request.ProviderReferenceId);
                return Result<bool>.Failure("An error occurred while processing the recharge failure", System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
