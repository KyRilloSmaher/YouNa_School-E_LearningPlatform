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
using YouNaSchhol.Users.Application.Features.Commands.AssignAssistantToStudent;
using YouNaSchool.Users.Application.Abstractions.Persistence;

namespace YouNaSchhol.Users.Application.Features.Commands.TransferStudentToAssistant
{
    public class TransferStudentToAssistantCommandHandler : IRequestHandler<TransferStudentToAssistantCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;
        private readonly IAssistantRepository _assistantRepository;
        private readonly ILogger<TransferStudentToAssistantCommandHandler> _logger;

        public TransferStudentToAssistantCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IStudentRepository studentRepository, IAssistantRepository assistantRepository, ILogger<TransferStudentToAssistantCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _studentRepository = studentRepository;
            _assistantRepository = assistantRepository;
            _logger = logger;
        }
        public async Task<Result<Guid>> Handle(TransferStudentToAssistantCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId);
            if (student == null)
            {
                return Result<Guid>.Failure("Student not found", System.Net.HttpStatusCode.NotFound);
            }
            var assistant = await _assistantRepository.GetByIdAsync(request.AssistantId);
            if (assistant == null)
            {
                return Result<Guid>.Failure("Assistant not found", System.Net.HttpStatusCode.NotFound);
            }
            student.AssignAssistant(assistant.Id);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Success(assistant.UserId);
        }
    }
}
