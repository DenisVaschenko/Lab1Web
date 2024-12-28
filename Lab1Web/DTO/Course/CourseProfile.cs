using AutoMapper;
using Lab1Web.Entities;
namespace Lab1Web.DTO
{
    public class CourseProfile: Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseOutputDto>()
                .ForMember(d => d.Students, opt => opt.MapFrom(src => src.Students))
                .ForMember(d => d.Instructors, opt => opt.MapFrom(src => src.Instructors));
            CreateMap<Course, CourseOutputDtoCollection>();
        }
    }
}
