using Lab1Web.Entities;

namespace Lab1Web.DTO
{
    public class CourseOutputDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<InstructorOutputDtoCollection> Instructors { get; set; }
        public int Duration { get; set; }
        public string Difficulty { get; set; }
        public List<StudentOutputDtoCollection> Students { get; set; }
    }
    public class CourseOutputDtoCollection
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
