using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.UNIT_OF_WORK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Users.Application.Abstractions.Persistence;

namespace YouNaSchhol.Users.Application.Features.Commands.AssignAssistantToStudent
{
    public class AssignAssistantToStudentCommandHandler : IRequestHandler<AssignAssistantToStudentCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;
        private readonly IAssistantRepository _assistantRepository;
        private readonly ILogger<AssignAssistantToStudentCommandHandler> _logger;

        public AssignAssistantToStudentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IStudentRepository studentRepository, IAssistantRepository assistantRepository, ILogger<AssignAssistantToStudentCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _studentRepository = studentRepository;
            _assistantRepository = assistantRepository;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(AssignAssistantToStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId ,true, cancellationToken);
            if (student == null)
            {
                _logger.LogWarning("Student with ID {StudentId} not found.", request.StudentId);
                return Result<Guid>.Failure("Student not found.", System.Net.HttpStatusCode.NotFound);
            }
            var assistant = await _assistantRepository.GetByIdAsync(request.AssistantId , false , cancellationToken);
            if (assistant == null)
            {
                _logger.LogWarning("Assistant with ID {AssistantId} not found.", request.AssistantId);
                return Result<Guid>.Failure("Assistant not found.", System.Net.HttpStatusCode.NotFound);
            }
            student.AssignAssistant(assistant.Id);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Assistant with ID {AssistantId} assigned to Student with ID {StudentId}.", request.AssistantId, request.StudentId);
            return Result<Guid>.Success(assistant.UserId);
        }
    }
}
