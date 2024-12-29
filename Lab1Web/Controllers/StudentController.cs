using AutoMapper;
using Lab1Web.Configuration;
using Lab1Web.DTO;
using Lab1Web.Entities;
using Lab1Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lab1Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IGenericRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentController> _logger;
        private readonly IOptions<StudentConfiguration> _options;
        public StudentController(IGenericRepository repository, IMapper mapper, ILogger<StudentController> logger, IOptionsSnapshot<StudentConfiguration> options)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
            _options = options;
        }

        /// <summary>
        /// Returns all students
        /// </summary>
        /// <param name="page"> The current page number.</param>
        /// <param name="pageSize">The desired page size.</param>
        /// <returns>Paged courses</returns>
        [HttpGet(Name = "GetAllStudents")]
        public async Task<IEnumerable<StudentOutputDto>> GetAll(int page = 1, int pageSize = 10) 
        {
            if (_options.Value.ApiMode == "write_only") return null;
            return await _mapper.ProjectTo<StudentOutputDto>(_repository.StudentRepository.GetAll().AsNoTracking().Skip((page - 1) * pageSize).Take(pageSize)).ToListAsync();
        }


        /// <summary>
        /// Returns Student for an id specified
        /// </summary>
        /// <param name="id"> Id of the student</param>
        /// <response code = "200">Returns the found item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpGet("{id}", Name = "Find student by id")]
        [OutputCache]
        [ProducesResponseType<Course>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<StudentOutputDto> GetById(int id) 
        {
            if (_options.Value.ApiMode == "write_only") return null;
            return _mapper.Map<StudentOutputDto>(await _repository.StudentRepository.FindAsync(id)); 
        }

        [HttpGet("find-by-name/{name}", Name = "Find student by name")]
        public async Task<StudentOutputDto> GetByName(string name)
        {
            if (_options.Value.ApiMode == "write_only") return null;
            return _mapper.Map<StudentOutputDto>(await _repository.StudentRepository.GetAll().AsNoTracking().FirstAsync(x => x.Name == name));
        }
            

        [HttpPost(Name = "AddStudent")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> Post([FromBody] Student student)
        {
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                await _repository.StudentRepository.AddAsync(student);
                _logger.LogInformation("Student with id {Id} was added", student.Id);
                return CreatedAtAction(nameof(GetAll), _mapper.Map<StudentOutputDto>(student));
            }
            catch
            {
                _logger.LogError("Error on attempt to add student");
                return BadRequest();
            }
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
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                var student = await _repository.StudentRepository.FindAsync(id);
                if (student == null) return NotFound();
                student.Name = newStudent.Name;
                student.Email = newStudent.Email;
                student.Phone = newStudent.Phone;
                student.Group = newStudent.Group;
                student.Age = newStudent.Age;
                student.AverageScore = newStudent.AverageScore;
                await _repository.StudentRepository.UpdateAsync(student);
                _logger.LogInformation("Student with id {Id} was changed", id);
                return Ok(_mapper.Map<StudentOutputDto>(student));
            }
            catch
            {
                _logger.LogError("Erroe on attempt to change student with id {Id}", id);
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
                var student = await _repository.StudentRepository.GetAll().Include(e => e.Courses).FirstAsync(e => e.Id == id);
                if (student == null) return NotFound();
                var courses = await _repository.CourseRepository.GetAll().Where(e => coursesId.Contains(e.Id)).Where(e => !student.Courses.Contains(e)).Take(_options.Value.MaxAttaching).ToListAsync();
                student.Courses.AddRange(courses);
                await _repository.StudentRepository.UpdateAsync(student);
                _logger.LogInformation("To the student with id {Id} were added courses", id);
                if (coursesId.Count() > courses.Count()) _logger.LogWarning("Some courses already were attached to the student with id {Id}", id);
                return Ok(_mapper.Map<StudentOutputDto>(student));
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to attach courses to the student with id {Id}", id);
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
                var student = await _repository.StudentRepository.GetAll().Include(e => e.Courses).FirstAsync(e => e.Id == id);
                if (student == null) return NotFound();
                var courses = await _repository.CourseRepository.GetAll().Where(e => coursesId.Contains(e.Id)).Take(_options.Value.MaxAttaching).ToListAsync();
                foreach (var e in courses)
                {
                    student.Courses.Remove(e);
                }
                await _repository.StudentRepository.UpdateAsync(student);
                _logger.LogInformation("From the student with id {Id} were removed courses", id);
                if (coursesId.Count() > courses.Count()) _logger.LogWarning("Some courses weren't attached to the student with id {Id}", id);
                return Ok(_mapper.Map<StudentOutputDto>(student));
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to detach courses from the student with id {Id}", id);
                return BadRequest();
            }
        }
        /// <summary>
        /// Delete 
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <response code = "200">Returns the deleted item</response>
        /// <response code = "404">If the item isn't found</response>
        [HttpDelete("{id}", Name = "Delete student by Id")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                var student = await _repository.StudentRepository.FindAsync(id);
                if (student == null) return NotFound();
                await _repository.StudentRepository.DeleteAsync(student);
                _logger.LogInformation("Student with id {Id} was removed", id);
                return Ok(_mapper.Map<StudentOutputDto>(student));
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to remove student with id {Id}", id);
                return BadRequest();
            }
        }
        [HttpDelete(Name = "Delete all students")]
        public async Task<IActionResult> Delete()
        {
            if (_options.Value.ApiMode == "read_only") return BadRequest("Action was denied");
            try
            {
                await _repository.StudentRepository.DeleteAsync();
                _logger.LogInformation("All students were removed");
                return Ok();
            }
            catch
            {
                _logger.LogInformation("Error in attpemt to remove all students");
                return BadRequest();
            }
        }
    }
}
