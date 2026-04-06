using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.UNIT_OF_WORK;
using System.Net;
using YouNaSchhol.Users.Application.Features.Commands.AssignAssistantToStudent;
using YouNaSchool.Users.Application.Abstractions.Persistence;

namespace YouNaSchhol.Users.Application.Features.Commands.RemoveAssistantFromStudent
{
    public class RemoveAssistantFromStudentCommandHandler : IRequestHandler<RemoveAssistantFromStudentCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;
        private readonly IAssistantRepository _assistantRepository;
        private readonly ILogger<RemoveAssistantFromStudentCommandHandler> _logger;

        public RemoveAssistantFromStudentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IStudentRepository studentRepository, IAssistantRepository assistantRepository, ILogger<RemoveAssistantFromStudentCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _studentRepository = studentRepository;
            _assistantRepository = assistantRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(RemoveAssistantFromStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, true,cancellationToken);
            if (student is null)
            {
                _logger.LogWarning("Student with ID {StudentId} not found.", request.StudentId);
                return Result<bool>.Failure("Student not found." ,code: HttpStatusCode.NotFound);
            }
            var assistant = await _assistantRepository.GetByIdAsync(request.AssistantId,false, cancellationToken);
            if (assistant is null)
            {
                _logger.LogWarning("Assistant with ID {AssistantId} not found.", request.AssistantId);
                return Result<bool>.Failure("Assistant not found.", code: HttpStatusCode.NotFound);
            }
            student.RemoveAssistant(assistant.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Assistant with ID {AssistantId} removed from Student with ID {StudentId}.", request.AssistantId, request.StudentId);
            return Result<bool>.Success(true);
        }
    }
}
