using Lab1Web;
using Lab1Web.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lab1Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly DataModelContext _context;
        public CourseController(DataModelContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Returns all courses
        /// </summary>
        /// <param name="page"> The current page number.</param>
        /// <param name="pageSize">The desired page size.</param>
        /// <returns>Paged courses</returns>
        [HttpGet(Name = "GetAllCourses")]
        public async Task<IEnumerable<Course>> GetAll(int page = 1, int pageSize = 10) =>
            await _context.Set<Course>().AsNoTracking().OrderBy(x => x.Id).Skip((page-1)*pageSize).Take(pageSize).ToListAsync();

        /// <summary>
        /// Returns Course for an id specified
        /// </summary>
        /// <param name="id"> Id of the course</param>
        [HttpGet("{id}", Name = "Find course by id")]
        public async Task<Course> GetById(int id) => await _context.Set<Course>().FindAsync(id);

        [HttpGet("find-by-title/{title}", Name = "Find course by title")]
        public async Task<Course> GetByTitle(string title) => await _context.Set<Course>().AsNoTracking().FirstAsync(x => x.Title == title);

        [HttpPost(Name = "AddCourse")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> Post([FromBody] Course course)
        {
            _context.Set<Course>().Add(course);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), course);
        }
        /// <summary>
        /// Update the existing entity by its id
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("{id}", Name = "Update course")]
        public async Task<IActionResult> Put(int id, Course newCourse) 
        {
            var course = await _context.Set<Course>().FindAsync(id);
            course.Title = newCourse.Title;
            course.Difficulty = newCourse.Difficulty;
            course.Duration = newCourse.Duration;
            await _context.SaveChangesAsync();
            return Ok(course);

        }/*
        /// <summary>
        /// Set new instructors to the course
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> AttachInstructors(int id, IEnumerable<int> instructorsId)
        {
            var course = await _context.Set<Course>().FindAsync(id);
            return Ok(course);
        }
        /// <summary>
        /// Set new students to the course
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> AttachStudents(int id, IEnumerable<int> studentsId)
        {
            var course = await _context.Set<Course>().FindAsync(id);
            return Ok(course);
        }
        /// <summary>
        /// Detach instructors from the course
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public Task<IActionResult> DetachInstructors(int id, IEnumerable<int> instructorsId)
        {
            var course = await _context.Set<Course>().FindAsync(id);
            return Ok(course);
        }
        /// <summary>
        /// Detach students from the course
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public IActionResult DetachStudents(int id, IEnumerable<int> studentsId)
        {
            var course = _storage.CourseStorage.Get(id);
            if (course == null) return NotFound();
            foreach (var item in studentsId.Where(x => course.StudentsId.Contains(x)))
            {
                course.StudentsId.Remove(item);
                _storage.StudentStorage.Get(item).CoursesId.Remove(id);
            };
            return Ok(course);
        }*/
        /// <summary>
        /// Delete 
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpDelete("{id}", Name ="Delete course by Id")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Set<Course>().FindAsync(id);
            _context.Set<Course>().Remove(course);
            await _context.SaveChangesAsync();
            return Ok(course);  
        }
        [HttpDelete(Name = "Delete all courses")]
        public async Task<IActionResult> Delete()
        {
            var list = await _context.Set<Course>().ToListAsync();
            _context.Set<Course>().RemoveRange(list);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}