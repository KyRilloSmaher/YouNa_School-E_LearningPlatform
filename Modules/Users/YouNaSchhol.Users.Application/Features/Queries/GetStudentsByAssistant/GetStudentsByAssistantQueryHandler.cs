using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.UNIT_OF_WORK;
using System.Net;
using YouNaSchhol.Users.Application.DTOs.Students;
using YouNaSchhol.Users.Application.Features.Queries.GetStudentsByAssistant.GetStudentsByAssistant;
using YouNaSchool.Users.Application.Abstractions.Persistence;

namespace YouNaSchhol.Users.Application.Features.Queries.GetStudentsByAssistant
{
    public class GetStudentsByAssistantQueryHandler : IRequestHandler<GetStudentsByAssistantQuery, Result<IEnumerable<StudentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;
        private readonly IAssistantRepository _assistantRepository;
        private readonly ILogger<GetStudentsByAssistantQueryHandler> _logger;

        public GetStudentsByAssistantQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IStudentRepository studentRepository, IAssistantRepository assistantRepository, ILogger<GetStudentsByAssistantQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _studentRepository = studentRepository;
            _assistantRepository = assistantRepository;
            _logger = logger;
        }
        public async Task<Result<IEnumerable<StudentDto>>> Handle(GetStudentsByAssistantQuery request, CancellationToken cancellationToken)
        {
            var assistant = await _assistantRepository.GetByIdAsync(request.AssistantId);
            if (assistant is null)
            { 
                _logger.LogWarning("Assistant with ID {AssistantId} not found.", request.AssistantId);
                return Result<IEnumerable<StudentDto>>.Failure($"Assistant with ID {request.AssistantId} not found.",HttpStatusCode.BadRequest);
            }
            var students = await _studentRepository.GetByAssistantIdAsync(request.AssistantId);
            if (students is null)
            {
                _logger.LogInformation("No students found for Assistant with ID {AssistantId}.", request.AssistantId);
                return Result<IEnumerable<StudentDto>>.Success(Enumerable.Empty<StudentDto>());
            }
            var studentDtos = _mapper.Map<IEnumerable<StudentDto>>(students);
            return Result<IEnumerable<StudentDto>>.Success(studentDtos);
        }
    }
}
