using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.RESULT_PATTERN;
using SharedKernel.Application.UNIT_OF_WORK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchhol.Users.Application.DTOs.Assistants;
using YouNaSchhol.Users.Application.Features.Commands.AssignAssistantToStudent;
using YouNaSchool.Users.Application.Abstractions.Persistence;
using YouNaSchool.Users.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace YouNaSchhol.Users.Application.Features.Queries.GetAvailableAssistants
{
    public class GetAvailableAssistantsQueryHandler : IRequestHandler<GetAvailableAssistantsQuery, Result<PaginatedResult<AssistantDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAssistantRepository _assistantRepository;
        private readonly ILogger<GetAvailableAssistantsQueryHandler> _logger;
        public GetAvailableAssistantsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,IAssistantRepository assistantRepository, ILogger<GetAvailableAssistantsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _assistantRepository = assistantRepository;
            _logger = logger;
        }

        public async Task<Result<PaginatedResult<AssistantDto>>> Handle(GetAvailableAssistantsQuery request, CancellationToken cancellationToken)
        {
            var query = await _assistantRepository.GetAvailableAssistanceForTeacherAsync(request.TeacherId, false, cancellationToken);
            var projectedQuery = _mapper.ProjectTo<AssistantDto>(query);
            var paginatedassistance = await projectedQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize);
            
            if (!paginatedassistance.Items.Any())
            {
                _logger.LogInformation("No available assistants found for teacher with ID {TeacherId}.", request.TeacherId);
            }


            return Result<PaginatedResult<AssistantDto>>.Success(paginatedassistance);
        }
    }
}
