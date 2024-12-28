using Lab1Web.Entities;

namespace Lab1Web.DTO
{
    public class CourseOutputDto
    {
        public int Id { get; }
        public string Title { get; set; }
        public List<InstructorOutputDtoCollection> Instructors { get; } = new List<InstructorOutputDtoCollection>();
        public int Duration { get; set; }
        public string Difficulty { get; set; }
        public List<StudentOutputDtoCollection> Students { get; } = new List<StudentOutputDtoCollection>();
    }
    public class CourseOutputDtoCollection
    {
        public int Id { get; }
        public string Title { get; }
    }
}
