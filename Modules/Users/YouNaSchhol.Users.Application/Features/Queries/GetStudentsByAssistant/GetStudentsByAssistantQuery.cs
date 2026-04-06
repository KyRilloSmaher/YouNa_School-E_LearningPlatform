using MediatR;
using Shared.Application.RESULT_PATTERN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchhol.Users.Application.DTOs.Students;

namespace YouNaSchhol.Users.Application.Features.Queries.GetStudentsByAssistant.GetStudentsByAssistant
{
    public record GetStudentsByAssistantQuery(Guid AssistantId) : IRequest<Result<IEnumerable<StudentDto>>>;
}
