using System.Text.Json.Serialization;
using System.Xml;

namespace Lab1Web.Entities
{
    public abstract class Person: Entity
    {
        public int Id { get; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<Course> CoursesId { get; } = new List<Course>();

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
