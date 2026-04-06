using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchhol.Users.Application.DTOs.Students;
using YouNaSchool.Users.Domain.Entities;

namespace YouNaSchhol.Users.Application.Mapping
{
    public class StudentaMapperProfile : Profile
    {
        public StudentaMapperProfile() {
            CreateMap<Student, StudentDto>();
        }
    }
}
