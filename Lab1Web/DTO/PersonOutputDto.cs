namespace Lab1Web.DTO
{
    public abstract class PersonOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public List<CourseOutputDtoCollection> Courses { get; set; }
    }
}
