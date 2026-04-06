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

namespace YouNaSchhol.Users.Application.Features.Commands.SuspendStudent
{
    public class SuspendStudnetCommandHandler : IRequestHandler<SuspendStudnetCommand, Result<bool>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SuspendStudnetCommandHandler> _logger;
        public SuspendStudnetCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork, ILogger<SuspendStudnetCommandHandler> logger)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<Result<bool>> Handle(SuspendStudnetCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId,true, cancellationToken);
            if (student is null)
            {
                _logger.LogWarning("Student with id {StudentId} not found", request.StudentId);
                return Result<bool>.Failure("Student not found", System.Net.HttpStatusCode.NotFound);
            }
            student.Suspend("Violation of school rules");
             await  _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Student with id {StudentId} has been suspended", request.StudentId);
            return Result<bool>.Success(true);
        }
    }
}
