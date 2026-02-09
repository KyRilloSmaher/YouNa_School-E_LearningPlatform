using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.UNIT_OF_WORK;
using SharedKernel.Domain.VALUE_OBJECTS;
using YouNaSchool.Wallet.Application.Abstractions.ExternalService.Payment;
using YouNaSchool.Wallet.Application.DTOs;
using YouNaSchool.Wallet.Domain.Entities;
using YouNaSchool.Wallet.Domain.Repositories;

namespace YouNaSchool.Wallet.Application.Commands.RechargeWallet
{
    public sealed class RechargeWalletCommandHandler: IRequestHandler<RechargeWalletCommand, Result<RechargeWalletResult>>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletRechargeRepository _rechargeRepository;
        private readonly IPaymentGatewayFactory _gatewayFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<RechargeWalletCommandHandler> _logger;
        private IUnitOfWork _unitOfWork;
        public RechargeWalletCommandHandler(
            IWalletRepository walletRepository,
            IWalletRechargeRepository rechargeRepository,
            IPaymentGatewayFactory gatewayFactory,
            IMapper mapper,
            ILogger<RechargeWalletCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _walletRepository = walletRepository;
            _rechargeRepository = rechargeRepository;
            _gatewayFactory = gatewayFactory;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RechargeWalletResult>> Handle(RechargeWalletCommand request,CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting wallet recharge. WalletId={WalletId}, Amount={Amount}, Provider={Provider}",
                request.WalletId,
                request.Amount,
                request.Provider
            );

            var wallet = await _walletRepository.GetByIdAsync(request.WalletId);

            if (wallet is null)
                throw new InvalidOperationException("Wallet not found");

            var money = new Money(request.Amount, request.Currency);

            // Create domain recharge
            var recharge = new WalletRecharge(
                walletId: wallet.Id,
                amount: money,
                provider: request.Provider,
                providerReferenceId: string.Empty,
                clientPaymentToken: null
            );

            await _rechargeRepository.AddAsync(recharge, cancellationToken);

            //Resolve payment gateway
            var gateway = _gatewayFactory.Resolve(request.Provider);

            // 3️⃣ Create payment session
            var session = await gateway.CreateSessionAsync(
                new CreatePaymentSessionCommand
                {
                    WalletId = wallet.Id,
                    RechargeId = recharge.Id,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    Provider = request.Provider,
                    CallbackUrl = request.CallbackUrl
                },
                cancellationToken
            );

            //Attach provider session to domain
            recharge.AttachProviderSession(
                session.ProviderReferenceId,
                session.ClientPaymentToken
            );

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Wallet recharge created successfully. RechargeId={RechargeId}, ProviderRef={ProviderRef}",
                recharge.Id,
                session.ProviderReferenceId
            );

            // 5️⃣ Return safe response
            return  Result<RechargeWalletResult>.Success(
                _mapper.Map<RechargeWalletResult>(recharge)
            );
        }
    }
}