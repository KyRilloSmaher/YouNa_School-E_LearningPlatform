using MediatR;
using Shared.Application.RESULT_PATTERN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Application.Features.Commands.RechargeCompleted
{
    public record RechargeCompletedCommand(Guid? RechargeId, string ProviderReferenceId) :IRequest<Result<bool>>;
}
