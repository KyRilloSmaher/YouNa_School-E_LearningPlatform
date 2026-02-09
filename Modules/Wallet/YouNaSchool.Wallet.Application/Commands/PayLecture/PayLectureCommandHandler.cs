using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.RESULT_PATTERN;
using SharedKernel.Application.UNIT_OF_WORK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Application.DTOs;
using YouNaSchool.Wallet.Domain.Enums;
using YouNaSchool.Wallet.Domain.Repositories;

namespace YouNaSchool.Wallet.Application.Commands.PayLecture
{
    public class PayLectureCommandHandler : IRequestHandler<PayLectureCommand, Result<PayLectureResultDTO>>
    {
        private readonly IWalletRepository _walletRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<PayLectureCommandHandler> _logger;
        private readonly IMapper _mapper;

        public PayLectureCommandHandler(IWalletRepository walletRepo, IUnitOfWork uow, ILogger<PayLectureCommandHandler> logger, IMapper mapper)
        {
            _walletRepo = walletRepo;
            _uow = uow;
            _logger = logger;
            _mapper = mapper;
        }


        public async Task<Result<PayLectureResultDTO>> Handle(PayLectureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Business logic to pay for a lecture
                var wallet = await _walletRepo.GetByIdAsync(request.LectureId,true);
                if (wallet == null)
                {
                    return Result<PayLectureResultDTO>.Failure("Wallet not found", System.Net.HttpStatusCode.NotFound);
                }
                wallet.Debit(request.Amount,WalletTransactionSource.LecturePayment, request.LectureId);
                await _uow.SaveChangesAsync();
                var resultDto = new PayLectureResultDTO
                {
                    WalletId = wallet.Id,
                    LectureId = request.LectureId,
                    AmountPaid = request.Amount.Amount,
                    PaymentDate = DateTime.UtcNow

                };
                return Result<PayLectureResultDTO>.Success(resultDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                return Result<PayLectureResultDTO>.Failure("An error occurred while processing the payment", System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
