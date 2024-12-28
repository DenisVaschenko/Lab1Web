using AutoMapper;
using Lab1Web;
using Lab1Web.DTO;
using Lab1Web.Entities;
using Lab1Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lab1Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IGenericRepository _repository;
        private readonly IMapper _mapper;
        public CourseController(IGenericRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository =  repository;
        }
        /// <summary>
        /// Returns all courses
        /// </summary>
        /// <param name="page"> The current page number.</param>
        /// <param name="pageSize">The desired page size.</param>
        /// <returns>Paged courses</returns>
        [HttpGet(Name = "GetAllCourses")]
        public async Task<IEnumerable<CourseOutputDto>> GetAll(int page = 1, int pageSize = 10) =>
            await _mapper.ProjectTo<CourseOutputDto>(_repository.CourseRepository.GetAll().AsNoTracking().Skip((page-1)*pageSize).Take(pageSize)).ToListAsync();

        /// <summary>
        /// Returns Course for an id specified
        /// </summary>
        /// <param name="id"> Id of the course</param>
        [HttpGet("{id}", Name = "Find course by id")]
        public async Task<CourseOutputDto?> GetById(int id) => 
            _mapper.Map<CourseOutputDto>(await _repository.CourseRepository.FindAsync(id));

        [HttpGet("find-by-title/{title}", Name = "Find course by title")]
        public async Task<CourseOutputDto?> GetByTitle(string title) => 
            _mapper.Map<CourseOutputDto>(await _repository.CourseRepository.GetAll().AsNoTracking().FirstAsync(x => x.Title == title));

        [HttpPost(Name = "AddCourse")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> Post([FromBody] Course course)
        {
            await _repository.CourseRepository.AddAsync(course);
            return CreatedAtAction(nameof(GetAll), _mapper.Map <CourseOutputDto>(course));
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
            var course = await _repository.CourseRepository.FindAsync(id);
            if (course == null) return NotFound();
            course.Title = newCourse.Title;
            course.Difficulty = newCourse.Difficulty;
            course.Duration = newCourse.Duration;
            await _repository.CourseRepository.UpdateAsync(course);
            return Ok(_mapper.Map<CourseOutputDto>(course));

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
            var course = await _repository.CourseRepository.GetAll().Include(e => e.Instructors).FirstAsync(e => e.Id == id);
            if (course == null) return NotFound();
            var instructors = await _repository.InstructorRepository.GetAll().Where(e => instructorsId.Contains(e.Id)).Where(e=>!course.Instructors.Contains(e)).ToListAsync();
            course.Instructors.AddRange(instructors);
            await _repository.CourseRepository.UpdateAsync(course);
            return Ok(_mapper.Map<CourseOutputDto>(course));
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
            var course = await _repository.CourseRepository.GetAll().Include(e => e.Students).FirstAsync(e => e.Id == id);
            if (course == null) return NotFound();
            var students = await _repository.StudentRepository.GetAll().Where(e => studentsId.Contains(e.Id)).Where(e => !course.Students.Contains(e)).ToListAsync();
            course.Students.AddRange(students);
            await _repository.CourseRepository.UpdateAsync(course);
            return Ok(_mapper.Map<CourseOutputDto>(course));
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
            var course = await _repository.CourseRepository.GetAll().Include(e => e.Instructors).FirstAsync(e => e.Id == id);
            if (course == null) return NotFound();
            var instructors = await _repository.InstructorRepository.GetAll().Where(e => instructorsId.Contains(e.Id)).ToListAsync();
            foreach (var e in instructors)
            {
                course.Instructors.Remove(e);
            }
            await _repository.CourseRepository.UpdateAsync(course);
            return Ok(_mapper.Map<CourseOutputDto>(course));
            
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
            var course = await _repository.CourseRepository.GetAll().Include(e => e.Students).FirstAsync(e => e.Id == id);
            if (course == null) return NotFound();
            var students = await _repository.StudentRepository.GetAll().Where(e => studentsId.Contains(e.Id)).ToListAsync();
            foreach (var e in students)
            {
                course.Students.Remove(e);
            }
            await _repository.CourseRepository.UpdateAsync(course);
            return Ok(_mapper.Map<CourseOutputDto>(course));
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
            var course = await _repository.CourseRepository.FindAsync(id);
            await _repository.CourseRepository.DeleteAsync(course);
            return Ok(_mapper.Map<CourseOutputDto>(course));  
        }
        [HttpDelete(Name = "Delete all courses")]
        public async Task<IActionResult> Delete()
        {
            await _repository.CourseRepository.DeleteAsync();
            return Ok();
        }
    }
}