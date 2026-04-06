using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchhol.Users.Application.DTOs.Teachers;
using YouNaSchool.Users.Domain.Entities;

namespace YouNaSchhol.Users.Application.Mapping
{
    public class TeacherMapperProfile : Profile
    {
        public TeacherMapperProfile()
        {
            CreateMap<Teacher,TeacherDto>();
        }
    }
}
