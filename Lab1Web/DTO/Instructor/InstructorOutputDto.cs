using Lab1Web.Entities;

namespace Lab1Web.DTO
{
    public class InstructorOutputDto : PersonOutputDto
    {
        public string? Specialisation { get; set; }
        public string Degree { get; set; }
    }
    public class InstructorOutputDtoCollection
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
