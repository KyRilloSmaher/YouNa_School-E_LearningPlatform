using AutoMapper;
using YouNaSchool.Wallet.Application.DTOs;
using YouNaSchool.Wallet.Domain.Entities;

namespace YouNaSchool.Wallet.Application.Mapping;

public sealed class WalletLedgerMappingProfile : Profile
{
    public WalletLedgerMappingProfile()
    {
        ConfigureMapFromWalletLedgerEntryToResponseDTO();
    }
    void ConfigureMapFromWalletLedgerEntryToResponseDTO()
    {
        CreateMap<WalletLedgerEntry, WalletLedgerEntryResponseDTO>()
         .ForMember(
             dest => dest.EntryId,
             opt => opt.MapFrom(src => src.Id)
         )
         .ForMember(
             dest => dest.Amount,
             opt => opt.MapFrom(src => src.Amount.Amount)
         )
         .ForMember(
             dest => dest.Currency,
             opt => opt.MapFrom(src => src.Amount.Currency)
         )
         .ForMember(
             dest => dest.TransactionType,
             opt => opt.MapFrom(src => src.TransactionType.ToString())
         )
         .ForMember(
             dest => dest.Source,
             opt => opt.MapFrom(src => src.Source.ToString())
         );
    }
}