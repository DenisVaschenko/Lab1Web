using Lab1Web.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lab1Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        Storage _storage;
        public InstructorController(Storage storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Returns all instructors
        /// </summary>
        /// <param name="page"> The current page number.</param>
        /// <param name="pageSize">The desired page size.</param>
        /// <returns>Paged courses</returns>
        [HttpGet(Name = "GetAllInstructors")]
        public IEnumerable<Instructor> GetAll(int page = 1, int pageSize = 10) =>
            _storage.InstructorStorage.GetAll().Skip(pageSize * (page - 1)).Take(pageSize);

        /// <summary>
        /// Returns Instructor for an id specified
        /// </summary>
        /// <param name="id"> Id of the Instructor</param>
        /// <response code = "200">Returns the found item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpGet("{id}", Name = "Find Instructor by id")]
        [ProducesResponseType<Course>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {

            var instructor = _storage.InstructorStorage.Get(id);
            return instructor != null ? Ok(instructor) : NotFound();
        }

        [HttpGet("find-by-name/{name}", Name = "Find Instructor by name")]
        public IActionResult GetByName(string name)
        {
            var instructors = _storage.InstructorStorage.GetAll().Where(x => x.Name.StartsWith(name));
            return instructors != null ? Ok(instructors) : NotFound();
        }

        [HttpPost(Name = "AddInstructor")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public IActionResult Post([FromBody] Instructor instructor) => CreatedAtAction(nameof(GetById), new { id = instructor.Id }, _storage.InstructorStorage.Add(instructor));
        /// <summary>
        /// Update the existing entity by its id
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("{id}", Name = "Update instructor")]
        public IActionResult Put(int id, string name, int age, string email, string phone)
        {
            var instructor = _storage.InstructorStorage.Get(id);
            if (instructor == null) return NotFound();
            if (name != null) instructor.Name = name;
            if (age != 0) instructor.Age = age;
            if (phone != null) instructor.Phone = phone;
            return Ok(instructor);

        }
        /// <summary>
        /// Set new courses
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public IActionResult AttachCourses(int id, IEnumerable<int> coursesId)
        {
            var instructor = _storage.InstructorStorage.Get(id);
            if (instructor == null) return NotFound();
            foreach (var item in coursesId.Where(x => !instructor.CoursesId.Contains(x) && _storage.CourseStorage.Get(x) != null))
            {
                instructor.CoursesId.Add(item);
                _storage.CourseStorage.Get(item).InstructorsId.Add(id);
            };
            return Ok(instructor);
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
            var instructor = _storage.InstructorStorage.Get(id);
            if (instructor == null) return NotFound();
            foreach (var item in coursesId.Where(x => instructor.CoursesId.Contains(x)))
            {
                instructor.CoursesId.Remove(item);
                _storage.CourseStorage.Get(item).InstructorsId.Remove(id);
            };
            return Ok(instructor);
        }
        /// <summary>
        /// Delete 
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the deleted item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpDelete("{id}", Name = "Delete instructor by Id")]
        public IActionResult Delete(int id)
        {
            var instructor = _storage.InstructorStorage.Get(id);
            if (instructor == null) return NotFound();
            foreach (var item in instructor.CoursesId.Select(x => _storage.CourseStorage.Get(x)))
            {
                item.InstructorsId.Remove(id);
            };
            _storage.InstructorStorage.Delete(id);
            return Ok(instructor);
        }
        [HttpDelete(Name = "Delete all instructors")]
        public IActionResult Delete()
        {
            foreach (var item in _storage.InstructorStorage.GetAll())
            {
                Delete(item.Id);
            }
            return Ok();
        }
    }
}
