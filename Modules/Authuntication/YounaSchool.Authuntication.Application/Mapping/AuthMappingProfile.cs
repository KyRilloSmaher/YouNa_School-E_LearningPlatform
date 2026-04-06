using AutoMapper;
using YounaSchool.Authuntication.Application.Abstractions.Persistence;
using YounaSchool.Authuntication.Application.DTOs;
using YounaSchool.Authuntication.Domain.Aggregates;

namespace YounaSchool.Authuntication.Application.Mapping;

public sealed class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<AuthUserInfo, UserDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName))
            .ForMember(d => d.Roles, o => o.MapFrom(s => new[] { s.Role }));

        CreateMap<AuthSession, SessionDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.DeviceInfo, o => o.MapFrom(s => s.DeviceInfo))
            .ForMember(d => d.IpAddress, o => o.MapFrom(s => s.IpAddress))
            .ForMember(d => d.CreatedAtUtc, o => o.MapFrom(s => s.CreatedAtUtc))
            .ForMember(d => d.RevokedAtUtc, o => o.MapFrom(s => s.RevokedAtUtc))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
