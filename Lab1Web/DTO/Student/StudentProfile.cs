using AutoMapper;
using Lab1Web.Entities;

namespace Lab1Web.DTO
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, StudentOutputDto>()
                .ForMember(d => d.Courses, opt => opt.MapFrom(src => src.Courses));
            CreateMap<Student, StudentOutputDtoCollection>();
        }
    }
}
