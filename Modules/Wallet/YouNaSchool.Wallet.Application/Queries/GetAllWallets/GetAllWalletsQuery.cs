using MediatR;
using Shared.Application.RESULT_PATTERN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Application.DTOs;

namespace YouNaSchool.Wallet.Application.Queries.GetAllWallets
{
    public record GetAllWalletsQuery(int pageNumber = 1 , int pageSize = 50): IRequest<Result<PaginatedResult<StudentWalletResponseDTO>>>;
}
