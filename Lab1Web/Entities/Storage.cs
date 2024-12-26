using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab1Web.Entities
{
    public interface IStorage<T> where T: Entity
    {
        public IEnumerable<T> GetAll();
        public T Get(int id);
        public T Add(T entity);
        public void DeleteAll();
        public void Delete(int id);
    }
    public class Storage<T> : IStorage<T> where T : Entity
    {
        protected static readonly List<T> Data = [];
        public IEnumerable<T> GetAll() => Data.OrderBy(x => x.Id);
        public T Get(int id) => Data.FirstOrDefault(x => x.Id == id);
        public T Add(T entity)
        {
            Data.Add(entity);
            return entity;
        }
        public void DeleteAll()
        {
            Data.Clear();
        }
        public void Delete(int id)
        {
            Data.RemoveAll(x => x.Id == id);
        }
    }
    public interface IStorage
    {
        public IStorage<Course> CourseStorage { get; set; }
        public IStorage<Student> StudentStorage { get; set; }
        public IStorage<Instructor> InstructorStorage { get; set; }
    }
    public class Storage: IStorage
    {
        public IStorage<Course> CourseStorage { get; set; } = new Storage<Course>();
        public IStorage<Student> StudentStorage { get; set; } = new Storage<Student>();
        public IStorage<Instructor> InstructorStorage { get; set; } = new Storage<Instructor>();
    }
        
}
