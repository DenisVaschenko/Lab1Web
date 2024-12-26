using Lab1Web;
using Lab1Web.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lab1Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        Storage _storage;
        public CourseController(Storage storage)
        {
            _storage = storage;
        }
        /// <summary>
        /// Returns all courses
        /// </summary>
        /// <param name="page"> The current page number.</param>
        /// <param name="pageSize">The desired page size.</param>
        /// <returns>Paged courses</returns>
        [HttpGet(Name = "GetAllCourses")]
        public IEnumerable<Course> GetAll(int page = 1, int pageSize = 10) =>
            _storage.CourseStorage.GetAll().Skip(pageSize * (page - 1)).Take(pageSize);

        /// <summary>
        /// Returns Course for an id specified
        /// </summary>
        /// <param name="id"> Id of the course</param>
        /// <response code = "200">Returns the found item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpGet("{id}", Name = "Find course by id")]
        [ProducesResponseType<Course>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var course = _storage.CourseStorage.Get(id);
            return course != null ? Ok(course) : NotFound();
        }

        [HttpGet("find-by-title/{title}", Name = "Find course by title")]
        public IActionResult GetByTitle(string title)
        {
            var courses = _storage.CourseStorage.GetAll().Where(x=>x.Title.StartsWith(title));
            return courses != null ? Ok(courses) : NotFound();
        }

        [HttpPost(Name = "AddCourse")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public IActionResult Post([FromBody] Course course) => CreatedAtAction(nameof(GetById),new { id = course.Id}, _storage.CourseStorage.Add(course));
        /// <summary>
        /// Update the existing entity by its id
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("{id}", Name = "Update course")]
        public IActionResult Put(int id, string title, string difficulty, int duration) 
        {
            var course = _storage.CourseStorage.Get(id);
            if (course == null) return NotFound();
            if (title != null) course.Title = title;
            if (difficulty != null) course.Difficulty = difficulty;
            if (duration != 0) course.Duration = duration;
            return Ok(course);

        }
        /// <summary>
        /// Set new instructors to the course
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public IActionResult AttachInstructors(int id, IEnumerable<int> instructorsId)
        {
            var course = _storage.CourseStorage.Get(id);
            if (course == null) return NotFound();
            
            foreach (var item in instructorsId.Where(x => !course.InstructorsId.Contains(x) && _storage.InstructorStorage.Get(x) != null))
            {
                course.InstructorsId.Add(item);
                _storage.InstructorStorage.Get(item).CoursesId.Add(id);
            };
            return Ok(course);
        }
        /// <summary>
        /// Set new students to the course
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public IActionResult AttachStudents(int id, IEnumerable<int> studentsId)
        {
            var course = _storage.CourseStorage.Get(id);
            if (course == null) return NotFound();
            foreach (var item in studentsId.Where(x => !course.StudentsId.Contains(x) && _storage.StudentStorage.Get(x) != null))
            {
                _storage.StudentStorage.Get(item).CoursesId.Add(id);
                course.StudentsId.Add(item);
            };
            return Ok(course);
        }
        /// <summary>
        /// Detach instructors from the course
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public IActionResult DetachInstructors(int id, IEnumerable<int> instructorsId)
        {
            var course = _storage.CourseStorage.Get(id);
            if (course == null) return NotFound();
            foreach (var item in instructorsId.Where(x => course.InstructorsId.Contains(x)))
            {
                course.InstructorsId.Remove(item);
                _storage.InstructorStorage.Get(item).CoursesId.Remove(id);
            };
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
        }
        /// <summary>
        /// Delete 
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpDelete("{id}", Name ="Delete course by Id")]
        public IActionResult Delete(int id)
        {
            var course = _storage.CourseStorage.Get(id);
            if (course == null) return NotFound();
            foreach (var item in course.StudentsId.Select(x => _storage.StudentStorage.Get(x)))
            {
                item.CoursesId.Remove(id);
            };
            foreach (var item in course.InstructorsId.Select(x => _storage.InstructorStorage.Get(x)))
            {
                item.CoursesId.Remove(id);
            };
            _storage.CourseStorage.Delete(id);
            return Ok(course);  
        }
        [HttpDelete(Name = "Delete all courses")]
        public IActionResult Delete()
        {
            foreach (var item in _storage.CourseStorage.GetAll())
            {
                Delete(item.Id);
            }
            return Ok();
        }
    }
}