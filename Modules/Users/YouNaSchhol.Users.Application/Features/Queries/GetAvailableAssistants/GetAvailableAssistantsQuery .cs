using MediatR;
using Shared.Application.RESULT_PATTERN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchhol.Users.Application.DTOs.Assistants;

namespace YouNaSchhol.Users.Application.Features.Queries.GetAvailableAssistants
{
    public record GetAvailableAssistantsQuery(Guid TeacherId, int PageNumber = 1 , int PageSize =20): IRequest<Result<PaginatedResult<AssistantDto>>>;
}
