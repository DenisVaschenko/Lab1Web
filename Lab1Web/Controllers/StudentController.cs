using Lab1Web.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lab1Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        Storage _storage;
        public StudentController(Storage storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Returns all students
        /// </summary>
        /// <param name="page"> The current page number.</param>
        /// <param name="pageSize">The desired page size.</param>
        /// <returns>Paged courses</returns>
        [HttpGet(Name = "GetAllStudents")]
        public IEnumerable<Student> GetAll(int page = 1, int pageSize = 10) =>
            _storage.StudentStorage.GetAll().Skip(pageSize * (page - 1)).Take(pageSize);

        /// <summary>
        /// Returns Student for an id specified
        /// </summary>
        /// <param name="id"> Id of the student</param>
        /// <response code = "200">Returns the found item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpGet("{id}", Name = "Find student by id")]
        [ProducesResponseType<Course>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            
            var student = _storage.StudentStorage.Get(id);
            return student != null ? Ok(student) : NotFound();
        }

        [HttpGet("find-by-name/{name}", Name = "Find student by name")]
        public IActionResult GetByName(string name)
        {
            var students = _storage.StudentStorage.GetAll().Where(x => x.Name.StartsWith(name));
            return students != null ? Ok(students) : NotFound();
        }

        [HttpPost(Name = "AddStudent")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public IActionResult Post([FromBody] Student student) => CreatedAtAction(nameof(GetById), new { id = student.Id }, _storage.StudentStorage.Add(student));
        /// <summary>
        /// Update the existing entity by its id
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("{id}", Name = "Update student")]
        public IActionResult Put(int id, string name, int age, string email, string phone)
        {
            var student = _storage.StudentStorage.Get(id);
            if (student == null) return NotFound();
            if (name != null) student.Name = name;
            if (age != 0) student.Age = age;
            if (phone != null) student.Phone = phone;
            return Ok(student);

        }
        /// <summary>
        /// Set new instructors to the course(-1 to not change the instructor)
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public IActionResult AttachCourses(int id, IEnumerable<int> coursesId)
        {
            var student = _storage.StudentStorage.Get(id);
            if (student == null) return NotFound();
            foreach (var item in coursesId.Where(x => !student.CoursesId.Contains(x) && _storage.CourseStorage.Get(x) != null))
            {
                student.CoursesId.Add(item);
            };
            return Ok(student);
        }
        /// <summary>
        /// Set new instructors to the course(-1 to not change the instructor)
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public IActionResult DetachCourses(int id, IEnumerable<int> coursesId)
        {
            var student = _storage.StudentStorage.Get(id);
            if (student == null) return NotFound();
            foreach (var item in student.CoursesId.Where(x => coursesId.Contains(x)))
            {
                student.CoursesId.Remove(item);
                _storage.CourseStorage.Get(item).StudentsId.Remove(id);
            };
            return Ok(student);
        }
        /// <summary>
        /// Delete 
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the deleted item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpDelete("{id}", Name = "Delete student by Id")]
        public IActionResult Delete(int id)
        {
            var student = _storage.StudentStorage.Get(id);
            if (student == null) return NotFound();
            student.CoursesId.Select(x => _storage.CourseStorage.Get(x).StudentsId.Remove(id));
            return Ok(student);
        }
        [HttpDelete(Name = "Delete all students")]
        public IActionResult Delete()
        {
            foreach (var item in _storage.StudentStorage.GetAll())
            {
                Delete(item.Id);
            }
            return Ok();
        }
    }
}
