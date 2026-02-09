using MediatR;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.RESULT_PATTERN;
using YouNaSchool.Wallet.Application.DTOs;

namespace YouNaSchool.Wallet.Application.Queries.GetWalletByStudentId
{
    public record GetWalletByStudentIdQuery (string StudentId) : IRequest<Result<StudentWalletResponseDTO>>;
}
