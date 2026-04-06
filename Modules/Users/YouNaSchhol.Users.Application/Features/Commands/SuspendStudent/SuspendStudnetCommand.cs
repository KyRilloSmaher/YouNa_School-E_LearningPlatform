using MediatR;
using Shared.Application.RESULT_PATTERN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchhol.Users.Application.Features.Commands.SuspendStudent
{
    public record SuspendStudnetCommand(Guid StudentId) : IRequest<Result<bool>>;
}
