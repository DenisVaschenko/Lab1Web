using FluentValidation;
using FluentValidation.Validators;
using Lab1Web.Entities;
namespace Lab1Web
{
    public class CourseValidator: AbstractValidator<Course>
    {
        public CourseValidator() 
        {
            RuleFor(x => x.Title).Length(2, 20);
            RuleFor(x => x.Difficulty).Length(3,10);
            RuleFor(x => x.Duration).GreaterThan(20).LessThan(300);
        }
    }
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Name).Length(5, 30);
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Phone).Matches(@"^\+?\d{10,15}$");
            RuleFor(x => x.Age).GreaterThan(16);
        }
    }
    public class StudentValidator : AbstractValidator<Student>
    {
        public StudentValidator()
        {
            Include(new PersonValidator());
            RuleFor(x => x.AverageScore).GreaterThan(59).LessThan(101);
            RuleFor(x => x.Group).Matches(@"^[A-Za-z]{2}-\d{2}$");
        }
    }
    public class InstructorValidator : AbstractValidator<Instructor>
    {
        public InstructorValidator()
        {
            Include(new PersonValidator());
            RuleFor(x => x.Degree).Length(5, 30);
            RuleFor(x => x.Specialisation).Length(5, 30);
        }
    }
}
