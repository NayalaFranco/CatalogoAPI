using System.Linq.Expressions;

namespace CatalogoAPI.Repository
{
    public interface IRepository<T>
    {
        // IQueryable é melhor nesse caso Linq-To-SQL
        IQueryable<T> Get();
        Task<T> GetById(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
