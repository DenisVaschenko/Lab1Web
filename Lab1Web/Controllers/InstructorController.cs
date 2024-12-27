using Lab1Web.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lab1Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly DataModelContext _context;
        public InstructorController(DataModelContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all instructors
        /// </summary>
        /// <param name="page"> The current page number.</param>
        /// <param name="pageSize">The desired page size.</param>
        /// <returns>Paged courses</returns>
        [HttpGet(Name = "GetAllInstructors")]
        public async Task<IEnumerable<Instructor>> GetAll(int page = 1, int pageSize = 10) =>
            await _context.Set<Instructor>().AsNoTracking().OrderBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        /// <summary>
        /// Returns Instructor for an id specified
        /// </summary>
        /// <param name="id"> Id of the Instructor</param>
        [HttpGet("{id}", Name = "Find Instructor by id")]
        public async Task<Instructor> GetById(int id) => await _context.Set<Instructor>().FindAsync(id);

        [HttpGet("find-by-name/{name}", Name = "Find Instructor by name")]
        public async Task<Instructor> GetByName(string name) => await _context.Set<Instructor>().AsNoTracking().FirstAsync(x => x.Name == name);

        [HttpPost(Name = "AddInstructor")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> Post([FromBody] Instructor instructor)
        {
            _context.Set<Instructor>().Add(instructor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), instructor);
        }
        /// <summary>
        /// Update the existing entity by its id
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("{id}", Name = "Update instructor")]
        public async Task<IActionResult> Put(int id, Instructor newInstructor)
        {
            Instructor instructor = await _context.Set<Instructor>().FindAsync(id);
            instructor.Name = newInstructor.Name;
            instructor.Email = newInstructor.Email;
            instructor.Phone = newInstructor.Phone;
            instructor.Specialisation = newInstructor.Specialisation;
            instructor.Age = newInstructor.Age;
            instructor.Degree = newInstructor.Degree;
            await _context.SaveChangesAsync();
            return Ok(instructor);

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
        }*/
        /// <summary>
        /// Delete 
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the deleted item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpDelete("{id}", Name = "Delete instructor by Id")]
        public async Task<IActionResult> Delete(int id)
        {
            Instructor instructor = await _context.Set<Instructor>().FindAsync(id);
            _context.Set<Instructor>().Remove(instructor);
            await _context.SaveChangesAsync();
            return Ok(instructor);
        }
        [HttpDelete(Name = "Delete all instructors")]
        public async Task<IActionResult> Delete()
        {
            var list = await _context.Set<Instructor>().ToListAsync();
            _context.Set<Instructor>().RemoveRange(list);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
