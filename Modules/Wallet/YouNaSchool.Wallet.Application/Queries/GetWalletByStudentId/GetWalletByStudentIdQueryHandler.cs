using AutoMapper;
using MediatR;
using Shared.Application.RESULT_PATTERN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Application.DTOs;
using YouNaSchool.Wallet.Domain.Repositories;

namespace YouNaSchool.Wallet.Application.Queries.GetWalletByStudentId
{
    public class GetWalletByStudentIdQueryHandler: IRequestHandler<GetWalletByStudentIdQuery,Result<StudentWalletResponseDTO>>
    {

        private readonly IMapper _mapper;
        private readonly IWalletRepository _walletRepository;

        public GetWalletByStudentIdQueryHandler(IWalletRepository walletRepository, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
        }

        public async Task<Result<StudentWalletResponseDTO>> Handle(GetWalletByStudentIdQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _walletRepository.GetByStudentIdAsync(request.StudentId);
            if (wallet is null)
                return Result<StudentWalletResponseDTO>.Failure("No Wallet Found For This Student ", HttpStatusCode.NotFound);
            var response = _mapper.Map<StudentWalletResponseDTO>(wallet);
            return Result<StudentWalletResponseDTO>.Success(response);
        }
    }
}
