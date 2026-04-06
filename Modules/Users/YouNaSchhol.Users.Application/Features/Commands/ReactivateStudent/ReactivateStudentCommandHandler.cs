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

namespace YouNaSchhol.Users.Application.Features.Commands.ReactivateStudent
{
    public class ReactivateStudentCommandHandler : IRequestHandler<ReactivateStudentCommand, Result<bool>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReactivateStudentCommandHandler> _logger;
        public ReactivateStudentCommandHandler(IStudentRepository studentRepository, ILogger<ReactivateStudentCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(ReactivateStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, true ,cancellationToken);
            if (student is null)
            {
                _logger.LogWarning("Student with ID {StudentId} not found.", request.StudentId);
                return Result<bool>.Failure("Student not found.", System.Net.HttpStatusCode.NotFound);
            }
            student.Reinstate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Student with ID {StudentId} reactivated successfully.", request.StudentId);
            return Result<bool>.Success(true);
        }
    }
}
