using MediatR;
using Shared.Application.RESULT_PATTERN;
namespace YouNaSchhol.Users.Application.Features.Commands.AssignAssistantToStudent
{
    public record AssignAssistantToStudentCommand(
     Guid StudentId,
     Guid AssistantId
    ):IRequest<Result<Guid>>;
}
