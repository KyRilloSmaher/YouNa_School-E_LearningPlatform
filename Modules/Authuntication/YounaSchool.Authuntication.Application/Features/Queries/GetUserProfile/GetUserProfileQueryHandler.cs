using AutoMapper;
using MediatR;
using Shared.Application.RESULT_PATTERN;
using System.Net;
using YounaSchool.Authuntication.Application.Abstractions.Persistence;
using YounaSchool.Authuntication.Application.DTOs;

namespace YounaSchool.Authuntication.Application.Features.Queries.GetUserProfile;

internal sealed class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserDto>>
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserProfileQueryHandler(IApplicationUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<UserDto>.Failure("User not found.", HttpStatusCode.NotFound);
        }

        var dto = _mapper.Map<UserDto>(user);
        return Result<UserDto>.Success(dto);
    }
}
