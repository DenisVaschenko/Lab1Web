using Lab1Web.Entities;

namespace Lab1Web.DTO
{
    public class StudentOutputDto : PersonOutputDto
    {
        public string? Specialisation { get; set; }
        public string Degree { get; set; }
    }
    public class StudentOutputDtoCollection
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
