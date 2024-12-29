using AutoMapper;
using Lab1Web.Entities;
using Lab1Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab1Web.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using Lab1Web.DTO;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.OutputCaching;
namespace Lab1Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly IGenericRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<InstructorController> _logger;
        private readonly IOptionsSnapshot<InstructorConfiguration> _options;
        public InstructorController(IGenericRepository repository, IMapper mapper, ILogger<InstructorController> logger, IOptionsSnapshot<InstructorConfiguration> options)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
            _options = options;
        }

        /// <summary>
        /// Returns all instructors
        /// </summary>
        /// <param name="page"> The current page number.</param>
        /// <param name="pageSize">The desired page size.</param>
        /// <returns>Paged courses</returns>
        [HttpGet(Name = "GetAllInstructors")]
        [OutputCache]
        public async Task<IEnumerable<InstructorOutputDto>> GetAll(int page = 1, int pageSize = 10) 
        {
            if (_options.Value.ApiMode == "write_only") return null;
            return await _mapper.ProjectTo<InstructorOutputDto>(_repository.InstructorRepository.GetAll().AsNoTracking().Skip((page - 1) * pageSize).Take(pageSize)).ToListAsync(); 
        }

        /// <summary>
        /// Returns Instructor for an id specified
        /// </summary>
        /// <param name="id"> Id of the Instructor</param>
        [HttpGet("{id}", Name = "Find Instructor by id")]
        public async Task<InstructorOutputDto> GetById(int id) 
        {
            if (_options.Value.ApiMode == "write_only") return null;
            return _mapper.Map<InstructorOutputDto>(await _repository.InstructorRepository.FindAsync(id)); 
        }

        [HttpGet("find-by-name/{name}", Name = "Find Instructor by name")]
        public async Task<InstructorOutputDto> GetByName(string name) => 
            _mapper.Map<InstructorOutputDto>(await _repository.InstructorRepository.GetAll().AsNoTracking().FirstAsync(x => x.Name == name));

        [HttpPost(Name = "AddInstructor")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> Post([FromBody] Instructor instructor)
        {
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                await _repository.InstructorRepository.AddAsync(instructor);
                _logger.LogInformation("Instructor with id {Id} was added", instructor.Id);
                return CreatedAtAction(nameof(GetAll), _mapper.Map<InstructorOutputDto>(instructor));
            }
            catch
            {
                _logger.LogError("Error on attempt to add instructor");
                return BadRequest();
            }
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
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
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
                _logger.LogInformation("Instructor with id {Id} was changed", id);
                return Ok(_mapper.Map<InstructorOutputDto>(instructor));
            }
            catch
            {
                _logger.LogError("Error on attempt to change instructor with id {Id}", id);
                return BadRequest();
            }

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
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                var instructor = await _repository.InstructorRepository.GetAll().Include(e => e.Courses).FirstAsync(e => e.Id == id);
                if (instructor == null) return NotFound();
                var courses = await _repository.CourseRepository.GetAll().Where(e => coursesId.Contains(e.Id)).Where(e => !instructor.Courses.Contains(e)).Take(_options.Value.MaxAttaching).ToListAsync();
                instructor.Courses.AddRange(courses);
                await _repository.InstructorRepository.UpdateAsync(instructor);
                _logger.LogInformation("To the instructor with id {Id} were added courses", id);
                if (coursesId.Count() > courses.Count()) _logger.LogWarning("Some courses already were attached to the instructor with id {Id}", id);
                return Ok(_mapper.Map<InstructorOutputDto>(instructor));
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to attach courses to the instructor with id {Id}", id);
                return BadRequest();
            }
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
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                var instructor = await _repository.InstructorRepository.GetAll().Include(e => e.Courses).FirstAsync(e => e.Id == id);
                if (instructor == null) return NotFound();
                var courses = await _repository.CourseRepository.GetAll().Where(e => coursesId.Contains(e.Id)).Take(_options.Value.MaxAttaching).ToListAsync();
                foreach (var e in courses)
                {
                    instructor.Courses.Remove(e);
                }
                await _repository.InstructorRepository.UpdateAsync(instructor);
                _logger.LogInformation("From the instructor with id {Id} were removed courses", id);
                if (coursesId.Count() > courses.Count()) _logger.LogWarning("Some courses already were attached to the instructor with id {Id}", id);
                return Ok(_mapper.Map<InstructorOutputDto>(instructor));
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to attach courses to the instructor with id {Id}", id);
                return BadRequest();
            }
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
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                var instructor = await _repository.InstructorRepository.FindAsync(id);
                if (instructor == null) return NotFound();
                await _repository.InstructorRepository.DeleteAsync(instructor);
                _logger.LogInformation("instructor with id {Id} was removed", id);
                return Ok(_mapper.Map<InstructorOutputDto>(instructor));
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to remove instructor with id {Id}", id);
                return BadRequest();
            }
        }
        [HttpDelete(Name = "Delete all instructors")]
        public async Task<IActionResult> Delete()
        {
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                await _repository.InstructorRepository.DeleteAsync();
                _logger.LogInformation("All instructors were removed");
                return Ok();
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to remove all instructors");
                return BadRequest();
            }
        }
    }
}
