using Lab1Web.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lab1Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly DataModelContext _context;
        public StudentController(DataModelContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all students
        /// </summary>
        /// <param name="page"> The current page number.</param>
        /// <param name="pageSize">The desired page size.</param>
        /// <returns>Paged courses</returns>
        [HttpGet(Name = "GetAllStudents")]
        public async Task<IEnumerable<Student>> GetAll(int page = 1, int pageSize = 10) =>
            await _context.Set<Student>().AsNoTracking().OrderBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        /// <summary>
        /// Returns Student for an id specified
        /// </summary>
        /// <param name="id"> Id of the student</param>
        /// <response code = "200">Returns the found item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpGet("{id}", Name = "Find student by id")]
        [ProducesResponseType<Course>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Student> GetById(int id) => await _context.Set<Student>().FindAsync(id);

        [HttpGet("find-by-name/{name}", Name = "Find student by name")]
        public async Task<Student> GetByName(string name) => await _context.Set<Student>().AsNoTracking().FirstAsync(x => x.Name == name);

        [HttpPost(Name = "AddStudent")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> Post([FromBody] Student student)
        {
            _context.Set<Student>().Add(student);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), student);
        }
        /// <summary>
        /// Update the existing entity by its id
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("{id}", Name = "Update student")]
        public async Task<IActionResult> Put(int id, Student newStudent)
        {
            Student student = await _context.Set<Student>().FindAsync(id);
            student.Name = newStudent.Name;
            student.Email = newStudent.Email;
            student.Phone = newStudent.Phone;
            student.Group = newStudent.Group;
            student.Age = newStudent.Age;
            student.AverageScore = newStudent.AverageScore;
            await _context.SaveChangesAsync();
            return Ok(student);

        }/*
        /// <summary>
        /// Set new courses
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
                _storage.CourseStorage.Get(item).StudentsId.Add(id);
            };
            return Ok(student);
        }
        /// <summary>
        /// Detach courses
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public IActionResult DetachCourses(int id, IEnumerable<int> coursesId)
        {
            var student = _storage.StudentStorage.Get(id);
            if (student == null) return NotFound();
            foreach (var item in coursesId.Where(x => student.CoursesId.Contains(x)))
            {
                student.CoursesId.Remove(item);
                _storage.CourseStorage.Get(item).StudentsId.Remove(id);
            };
            return Ok(student);
        }*/
        /// <summary>
        /// Delete 
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the deleted item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpDelete("{id}", Name = "Delete student by Id")]
        public async Task<IActionResult> Delete(int id)
        {
            Student student = await _context.Set<Student>().FindAsync(id);
            _context.Set<Student>().Remove(student);
            await _context.SaveChangesAsync();
            return Ok(student);
        }
        [HttpDelete(Name = "Delete all students")]
        public async Task<IActionResult> Delete()
        {
            var list = await _context.Set<Student>().ToListAsync();
            _context.Set<Student>().RemoveRange(list);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
