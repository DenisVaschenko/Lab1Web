using AutoMapper;
using Lab1Web.Entities;
namespace Lab1Web.DTO
{
    public class InstructorProfile:Profile
    {
        public InstructorProfile()
        {
            CreateMap<Instructor, InstructorOutputDto>().
                ForMember(d => d.Courses, opt => opt.MapFrom(src => src.Courses));
            CreateMap<Instructor, InstructorOutputDtoCollection>();
        }
    }
}
