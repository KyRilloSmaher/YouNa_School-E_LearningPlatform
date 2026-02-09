using MediatR;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Domain.VALUE_OBJECTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Application.DTOs;

namespace YouNaSchool.Wallet.Application.Commands.PayLecture
{
    public record PayLectureCommand(Guid WalletId,Guid StudentId , Guid LectureId, Money Amount) : IRequest<Result<PayLectureResultDTO>>;
}
