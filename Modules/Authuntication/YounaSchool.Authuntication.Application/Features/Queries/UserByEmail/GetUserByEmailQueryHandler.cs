using AutoMapper;
using MediatR;
using Shared.Application.RESULT_PATTERN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YounaSchool.Authuntication.Application.Abstractions.Persistence;
using YounaSchool.Authuntication.Application.DTOs;
using YounaSchool.Authuntication.Application.Features.Queries.UserEmailQuery;

namespace YounaSchool.Authuntication.Application.Features.Queries.UserByEmail
{
    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, Result<UserDto>>
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetUserByEmailQueryHandler(IApplicationUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<Result<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var user = _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
            {
                return Result<UserDto>.Failure("User not found", System.Net.HttpStatusCode.NotFound);
            }
            var userDto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Success(userDto);
        }
    }
}
