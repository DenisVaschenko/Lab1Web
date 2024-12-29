using AutoMapper;
using Lab1Web;
using Lab1Web.Configuration;
using Lab1Web.DTO;
using Lab1Web.Entities;
using Lab1Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lab1Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IGenericRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CourseController> _logger;
        private readonly IOptionsSnapshot<CourseConfiguration> _options;
        public CourseController(IGenericRepository repository, IMapper mapper, ILogger<CourseController> logger, IOptionsSnapshot<CourseConfiguration> options)
        {
            _mapper = mapper;
            _repository =  repository;
            _logger = logger;
            _options = options;
        }
        /// <summary>
        /// Returns all courses
        /// </summary>
        /// <param name="page"> The current page number.</param>
        /// <param name="pageSize">The desired page size.</param>
        /// <returns>Paged courses</returns>
        [HttpGet(Name = "GetAllCourses")]
        [OutputCache]
        public async Task<IEnumerable<CourseOutputDto>> GetAll(int page = 1, int pageSize = 10) 
        {
            if (_options.Value.ApiMode == "write_only") return null;
            return await _mapper.ProjectTo<CourseOutputDto>(_repository.CourseRepository.GetAll().AsNoTracking().Skip((page - 1) * pageSize).Take(pageSize)).ToListAsync(); 
        }


        /// <summary>
        /// Returns Course for an id specified
        /// </summary>
        /// <param name="id"> Id of the course</param>
        [HttpGet("{id}", Name = "Find course by id")]
        public async Task<CourseOutputDto?> GetById(int id)
        {
            if (_options.Value.ApiMode == "write_only") return null;
            return _mapper.Map<CourseOutputDto>(await _repository.CourseRepository.FindAsync(id));
        }

        [HttpGet("find-by-title/{title}", Name = "Find course by title")]
        public async Task<CourseOutputDto?> GetByTitle(string title)
        {
            if (_options.Value.ApiMode == "write_only") return null;
            try
            {
                return _mapper.Map<CourseOutputDto>(await _repository.CourseRepository.GetAll().AsNoTracking().FirstAsync(x => x.Title == title));
            }
            catch
            {
                _logger.LogError("Error on attempt to find Course by title");
                return null;
            }
        }

        [HttpPost(Name = "AddCourse")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> Post([FromBody] Course course)
        {
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                await _repository.CourseRepository.AddAsync(course);
                _logger.LogInformation("Course with id {Id} was added", course.Id);
                return CreatedAtAction(nameof(GetAll), _mapper.Map<CourseOutputDto>(course));
            }
            catch
            {
                _logger.LogError("Error on attempt to add Course");
                return BadRequest();
            }
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
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                var course = await _repository.CourseRepository.FindAsync(id);
                if (course == null) return NotFound();
                course.Title = newCourse.Title;
                course.Difficulty = newCourse.Difficulty;
                course.Duration = newCourse.Duration;
                await _repository.CourseRepository.UpdateAsync(course);
                _logger.LogInformation( "Course with id {Id} was changed", id);
                return Ok(_mapper.Map<CourseOutputDto>(course));
            }
            catch
            {
                _logger.LogError("Erroe on attempt to change course with id {Id}", id);
                return BadRequest();
            }

        }
        ///<summary>
        /// Set new instructors to the course
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> AttachInstructors(int id, IEnumerable<int> instructorsId)
        {
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                var course = await _repository.CourseRepository.GetAll().Include(e => e.Instructors).FirstAsync(e => e.Id == id);
                if (course == null) return NotFound();
                var instructors = await _repository.InstructorRepository.GetAll().Where(e => instructorsId.Contains(e.Id)).Where(e => !course.Instructors.Contains(e)).Take(_options.Value.MaxAttaching).ToListAsync();
                course.Instructors.AddRange(instructors);
                await _repository.CourseRepository.UpdateAsync(course);
                _logger.LogInformation("To the course with id {Id} were added instructors", id);
                if (instructorsId.Count() > instructors.Count()) _logger.LogWarning("Some instructors already were attached to the course with id {Id}",id);
                return Ok(_mapper.Map<CourseOutputDto>(course));
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to attach instructors to the course with id {Id}", id);
                return BadRequest();
            }
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
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                var course = await _repository.CourseRepository.GetAll().Include(e => e.Students).FirstAsync(e => e.Id == id);
                if (course == null) return NotFound();
                var students = await _repository.StudentRepository.GetAll().Where(e => studentsId.Contains(e.Id)).Where(e => !course.Students.Contains(e)).Take(_options.Value.MaxAttaching).ToListAsync();
                course.Students.AddRange(students);
                await _repository.CourseRepository.UpdateAsync(course);
                _logger.LogInformation("To the course with id {Id} were added students", id);
                if (studentsId.Count() > students.Count()) _logger.LogWarning("Some students already were attached to the course with id {Id}", id);
                return Ok(_mapper.Map<CourseOutputDto>(course));
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to attach students to the course with id {Id}", id);
                return BadRequest();
            }
        }
        /// <summary>
        /// Detach instructors from the course
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> DetachInstructors(int id, IEnumerable<int> instructorsId)
        {
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                var course = await _repository.CourseRepository.GetAll().Include(e => e.Instructors).FirstAsync(e => e.Id == id);
                if (course == null) return NotFound();
                var instructors = await _repository.InstructorRepository.GetAll().Where(e => instructorsId.Contains(e.Id)).Take(_options.Value.MaxAttaching).ToListAsync();
                foreach (var e in instructors)
                {
                    course.Instructors.Remove(e);
                }
                await _repository.CourseRepository.UpdateAsync(course);
                _logger.LogInformation("From the course with id {Id} were removed instructors", id);
                if (instructorsId.Count() > instructors.Count()) _logger.LogWarning("Some instructors weren't attached to the course with id {Id}", id);
                return Ok(_mapper.Map<CourseOutputDto>(course));
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to remove instructors from the course with id {Id}", id);
                return BadRequest();
            }
        }
        /// <summary>
        /// Detach students from the course
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> DetachStudents(int id, IEnumerable<int> studentsId)
        {
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                var course = await _repository.CourseRepository.GetAll().Include(e => e.Students).FirstAsync(e => e.Id == id);
                if (course == null) return NotFound();
                var students = await _repository.StudentRepository.GetAll().Where(e => studentsId.Contains(e.Id)).Take(_options.Value.MaxAttaching).ToListAsync();
                foreach (var e in students)
                {
                    course.Students.Remove(e);
                }
                await _repository.CourseRepository.UpdateAsync(course);
                if (studentsId.Count() > students.Count()) _logger.LogWarning("Some students weren't attached to the course with id {Id}", id);
                _logger.LogInformation("From the course with id {Id} were removed students", id);
                return Ok(_mapper.Map<CourseOutputDto>(course));
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to remove students from the course with id {Id}", id);
                return BadRequest();
            }
        }
        /// <summary>
        /// Delete 
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the changed item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpDelete("{id}", Name ="Delete course by Id")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                var course = await _repository.CourseRepository.FindAsync(id);
                await _repository.CourseRepository.DeleteAsync(course);
                _logger.LogInformation("Course with id {Id} was removed", id);
                return Ok(_mapper.Map<CourseOutputDto>(course));
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to remove course with id {Id}", id);
                return BadRequest();
            }
        }
        [HttpDelete(Name = "Delete all courses")]
        public async Task<IActionResult> Delete()
        {
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                await _repository.CourseRepository.DeleteAsync();
                _logger.LogInformation("All courses were removed");
                return Ok();
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to remove all courses");
                return BadRequest();
            }
        }
    }
}