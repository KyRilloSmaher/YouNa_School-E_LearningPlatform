using AutoMapper;
using MediatR;
using Shared.Application.RESULT_PATTERN;
using YouNaSchool.Wallet.Application.DTOs;
using YouNaSchool.Wallet.Domain.Repositories;

namespace YouNaSchool.Wallet.Application.Queries.GetAllWallets
{
    public class GetAllWalletsQueryHandler : IRequestHandler<GetAllWalletsQuery, Result<PaginatedResult<StudentWalletResponseDTO>>>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;
        public GetAllWalletsQueryHandler(IWalletRepository walletRepository, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
        }
        public async Task<Result<PaginatedResult<StudentWalletResponseDTO>>> Handle(GetAllWalletsQuery request, CancellationToken cancellationToken)
        {
            var query = await _walletRepository.GetAllAsQueryableAsync();
            var projectedQuery = _mapper.ProjectTo<StudentWalletResponseDTO>(query);
            var paginatedWallets = await projectedQuery.ToPaginatedListAsync(request.pageNumber, request.pageSize);
            return Result<PaginatedResult<StudentWalletResponseDTO>>.Success(paginatedWallets);
        }
    }
}
