using Lab1Web.Entities;
using Lab1Web.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Lab1Web.Repositories
{
    public interface IGenericRepository<TEntity, TKey> where TEntity:  class,Entity
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity?> FindAsync(TKey key);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync();
    }
    public class GenericRepository<TEntity, TKey>: IGenericRepository<TEntity, TKey> where TEntity : class, Entity
    {
        protected readonly DataModelContext _context;
        public GenericRepository(DataModelContext context) { _context = context; }
        public IQueryable<TEntity> GetAll() => _context.Set<TEntity>().OrderBy(x => x.Id);
        public Task<TEntity?> FindAsync(TKey key) => _context.Set<TEntity>().FindAsync(key).AsTask();
        public Task AddAsync(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            return _context.SaveChangesAsync();
        }
        public Task UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);

            return _context.SaveChangesAsync();
        }
        public Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);

            return _context.SaveChangesAsync();
        }
        public Task DeleteAsync()
        {
            _context.Set<TEntity>().RemoveRange(GetAll());
            return _context.SaveChangesAsync();
        }
    }
    public interface IGenericRepository
    {
        IGenericRepository<Course, int> CourseRepository { get;  }
        IGenericRepository<Instructor, int> InstructorRepository { get; }
        IGenericRepository<Student, int> StudentRepository { get; }

    }
    public class GenericRepository : IGenericRepository
    {
        public GenericRepository(DataModelContext context) 
        {
            CourseRepository = new GenericRepository<Course, int>(context);
            InstructorRepository = new GenericRepository<Instructor, int>(context);
            StudentRepository = new GenericRepository<Student, int>(context);
        }
        public IGenericRepository<Course, int> CourseRepository { get; }
        public IGenericRepository<Instructor, int> InstructorRepository { get; }
        public IGenericRepository<Student, int> StudentRepository { get; }
    }
}
public static class RepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services) => services
        .AddTransient<IGenericRepository, GenericRepository>();
}
