using AutoMapper;
using YouNaSchool.Wallet.Application.DTOs;
using YouNaSchool.Wallet.Domain.Entities;

namespace YouNaSchool.Wallet.Application.Mapping;

public sealed class WalletMappingProfile : Profile
{
    public WalletMappingProfile()
    {
        CreateMapFromWalletToResponse();
    }

    public void CreateMapFromWalletToResponse()
    {
        CreateMap<Wallets, StudentWalletResponseDTO>()
            .ForMember(
                dest => dest.WalletId,
                opt => opt.MapFrom(src => src.Id)
            )
            .ForMember(
                dest => dest.Balance,
                opt => opt.MapFrom(src => src.Balance.Amount)
            )
            .ForMember(
                dest => dest.Currency,
                opt => opt.MapFrom(src => src.Balance.Currency)
            );
    }
}