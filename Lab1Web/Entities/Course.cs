using System.Text.Json.Serialization;

namespace Lab1Web.Entities
{
    enum Persons
    {
        student,
        instructor
    }
    public class Course: Entity
    {
        public int Id { get; } = IdGenerator<Course>.Instance.Genereate();
        public string Title { get; set; }
        public List<int> InstructorsId { get; } = new List<int>();
        public int Duration { get; set; }
        public string Difficulty { get; set; }
        public List<int> StudentsId { get; } = new List<int>();
    }
}
