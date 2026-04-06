using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Application.Abstractions.ExternalService.Payment;
using YouNaSchool.Wallet.Application.DTOs;
using YouNaSchool.Wallet.Application.Features.Commands.RechargeWallet;
using YouNaSchool.Wallet.Domain.Entities;

namespace YouNaSchool.Wallet.Application.Mapping
{
    public class RechargeWalletMappingProfile : Profile
    {
        public RechargeWalletMappingProfile()
        {
            CreateMap<RechargeWalletCommand, CreatePaymentSessionCommand>();
            CreateMap<WalletRecharge, RechargeWalletResult>();
        }
    }

}
