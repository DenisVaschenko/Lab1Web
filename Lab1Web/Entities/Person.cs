using System.Text.Json.Serialization;
using System.Xml;

namespace Lab1Web.Entities
{
    public abstract class Person: Entity
    {
        public int Id { get; } = IdGenerator<Person>.Instance.Genereate();
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<int> CoursesId { get; } = new List<int>();

    }
    public class Student : Person
    {
        public string Group { get; set; }
        public double AverageScore { get; set; }

    }
    public class Instructor : Person
    {
        public string Specialisation { get; set; }
        public string Degree { get; set; }
    }

}
