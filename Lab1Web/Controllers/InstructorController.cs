using AutoMapper;
using Lab1Web.Entities;
using Lab1Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using Lab1Web.DTO;
namespace Lab1Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly IGenericRepository _repository;
        private readonly IMapper _mapper;
        public InstructorController(IGenericRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        /// <summary>
        /// Returns all instructors
        /// </summary>
        /// <param name="page"> The current page number.</param>
        /// <param name="pageSize">The desired page size.</param>
        /// <returns>Paged courses</returns>
        [HttpGet(Name = "GetAllInstructors")]
        public async Task<IEnumerable<InstructorOutputDto>> GetAll(int page = 1, int pageSize = 10) =>
            await _mapper.ProjectTo<InstructorOutputDto>(_repository.InstructorRepository.GetAll().AsNoTracking().Skip((page - 1) * pageSize).Take(pageSize)).ToListAsync();

        /// <summary>
        /// Returns Instructor for an id specified
        /// </summary>
        /// <param name="id"> Id of the Instructor</param>
        [HttpGet("{id}", Name = "Find Instructor by id")]
        public async Task<InstructorOutputDto> GetById(int id) => _mapper.Map<InstructorOutputDto>(await _repository.InstructorRepository.FindAsync(id));

        [HttpGet("find-by-name/{name}", Name = "Find Instructor by name")]
        public async Task<InstructorOutputDto> GetByName(string name) => 
            _mapper.Map<InstructorOutputDto>(await _repository.InstructorRepository.GetAll().AsNoTracking().FirstAsync(x => x.Name == name));

        [HttpPost(Name = "AddInstructor")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> Post([FromBody] Instructor instructor)
        {
            await _repository.InstructorRepository.AddAsync(instructor);
            return CreatedAtAction(nameof(GetAll), _mapper.Map<InstructorOutputDto>(instructor));
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
            var instructor = await _repository.InstructorRepository.FindAsync(id);
            if (instructor == null) return NotFound();
            instructor.Name = newInstructor.Name;
            instructor.Email = newInstructor.Email;
            instructor.Phone = newInstructor.Phone;
            instructor.Specialisation = newInstructor.Specialisation;
            instructor.Age = newInstructor.Age;
            instructor.Degree = newInstructor.Degree;
            await _repository.InstructorRepository.UpdateAsync(instructor);
            return Ok(_mapper.Map<InstructorOutputDto>(instructor));

        }
        /// <summary>
        /// Set new courses
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> AttachCourses(int id, IEnumerable<int> coursesId)
        {
            var instructor = await _repository.InstructorRepository.GetAll().Include(e => e.Courses).FirstAsync(e => e.Id == id);
            if (instructor == null) return NotFound();
            var courses = await _repository.CourseRepository.GetAll().Where(e => coursesId.Contains(e.Id)).Where(e => !instructor.Courses.Contains(e)).ToListAsync();
            instructor.Courses.AddRange(courses);
            await _repository.InstructorRepository.UpdateAsync(instructor);
            return Ok(_mapper.Map<InstructorOutputDto>(instructor));
        }
        /// <summary>
        /// Detach courses
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> DetachCourses(int id, IEnumerable<int> coursesId)
        {
            var instructor = await _repository.InstructorRepository.GetAll().Include(e => e.Courses).FirstAsync(e => e.Id == id);
            if (instructor == null) return NotFound();
            var courses = await _repository.CourseRepository.GetAll().Where(e => coursesId.Contains(e.Id)).ToListAsync();
            foreach (var e in courses)
            {
                instructor.Courses.Remove(e);
            }
            await _repository.InstructorRepository.UpdateAsync(instructor);
            return Ok(_mapper.Map<InstructorOutputDto>(instructor));
        }
        /// <summary>
        /// Delete 
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the deleted item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpDelete("{id}", Name = "Delete instructor by Id")]
        public async Task<IActionResult> Delete(int id)
        {
            var instructor = await _repository.InstructorRepository.FindAsync(id);
            if (instructor == null) return NotFound();
            await _repository.InstructorRepository.DeleteAsync(instructor);
            return Ok(_mapper.Map<InstructorOutputDto>(instructor));
        }
        [HttpDelete(Name = "Delete all instructors")]
        public async Task<IActionResult> Delete()
        {
            await _repository.InstructorRepository.DeleteAsync();
            return Ok();
        }
    }
}
