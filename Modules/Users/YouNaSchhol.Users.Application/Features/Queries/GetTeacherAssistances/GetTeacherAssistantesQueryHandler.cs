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
using YouNaSchhol.Users.Application.DTOs.Assistants;
using YouNaSchool.Users.Application.Abstractions.Persistence;

namespace YouNaSchhol.Users.Application.Features.Queries.GetTeacherAssistances
{
    public class GetTeacherAssistantesQueryHandler : IRequestHandler<GetTeacherAssistancesQuery, Result<PaginatedResult<AssistantDto>>>
    {
        private readonly IAssistantRepository _assistantRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTeacherAssistantesQueryHandler> _logger;

        public GetTeacherAssistantesQueryHandler(IAssistantRepository assistantRepository, IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTeacherAssistantesQueryHandler> logger)
        {
            _assistantRepository = assistantRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PaginatedResult<AssistantDto>>> Handle(GetTeacherAssistancesQuery request, CancellationToken cancellationToken)
        {
            var query = await _assistantRepository.GetByTeacherIdAsync(request.TeacherId);
            var projectedQuery = _mapper.ProjectTo<AssistantDto>(query);
            var paginatedassistance = await projectedQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize);

            if (!paginatedassistance.Items.Any())
            {
                _logger.LogInformation("No  assistants found for teacher with ID {TeacherId}.", request.TeacherId);
            }


            return Result<PaginatedResult<AssistantDto>>.Success(paginatedassistance);
        }
    }
}
