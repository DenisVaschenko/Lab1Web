using System.Text.Json.Serialization;

namespace Lab1Web.Entities
{
    public class Course: Entity
    {
        public int Id { get; }
        public string Title { get; set; }
        public List<Instructor> Instructors { get; } = new List<Instructor>();
        public int Duration { get; set; }
        public string Difficulty { get; set; }
        public List<Student> StudentsId { get; } = new List<Student>();
    }
}
