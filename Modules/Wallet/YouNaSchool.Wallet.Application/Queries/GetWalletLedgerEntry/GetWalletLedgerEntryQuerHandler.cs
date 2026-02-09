using AutoMapper;
using MediatR;
using Shared.Application.RESULT_PATTERN;
using System.Net;
using YouNaSchool.Wallet.Application.Abstractions.Persistence;
using YouNaSchool.Wallet.Application.DTOs;
using YouNaSchool.Wallet.Domain.Repositories;
namespace YouNaSchool.Wallet.Application.Queries.GetWalletLedgerEntry
{
    public class GetWalletLedgerEntryQuerHandler : IRequestHandler<GetWalletLedgerEntryQuery, Result<IEnumerable<WalletLedgerEntryResponseDTO>>>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletLedgerEntryRepository _walletLedgerEntryRepository;
        private readonly IMapper _mapper;

        public GetWalletLedgerEntryQuerHandler(IWalletRepository walletRepository, IWalletLedgerEntryRepository walletLedgerEntryRepository, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _walletLedgerEntryRepository = walletLedgerEntryRepository;
            _mapper = mapper;
        }
        public async Task<Result<IEnumerable<WalletLedgerEntryResponseDTO>>> Handle(GetWalletLedgerEntryQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return Result<IEnumerable<WalletLedgerEntryResponseDTO>>.Failure("Request cannot be null", HttpStatusCode.BadRequest);
            }
            var wallet = await _walletRepository.GetByStudentIdAsync(request.StudentId);
            if (wallet == null)
            {
                return Result<IEnumerable<WalletLedgerEntryResponseDTO>>.Failure("Wallet not found for the given student ID", HttpStatusCode.NotFound);
            }
            var ledgerEntries = await _walletLedgerEntryRepository.GetByWalletIdAsync(wallet.Id);
            var ledgerEntryDTOs = _mapper.Map<IEnumerable<WalletLedgerEntryResponseDTO>>(ledgerEntries);
            return Result<IEnumerable<WalletLedgerEntryResponseDTO>>.Success(ledgerEntryDTOs);
        }
    }
}
