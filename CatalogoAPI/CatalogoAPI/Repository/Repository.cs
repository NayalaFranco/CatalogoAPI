using CatalogoAPI.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CatalogoAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected CatalogoAPIContext _context;

        public Repository(CatalogoAPIContext context)
        {
            _context = context;
        }

        public IQueryable<T> Get()
        {
            return _context.Set<T>().AsNoTracking();
        }

        // O predicate é um delegate que verifica se o parametro
        // atende a uma condição especifica ou não.
        public T GetById(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().AsNoTracking().SingleOrDefault(predicate);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            // não precisa exatamente desse entity state, é mais para reforçar
            // pois ele ja esta sendo rastreado e o EF ja vai saber que
            // ele está modificado, mas reforçando garante melhor que sabe.
            _context.Entry(entity).State = EntityState.Modified;
            _context.Set<T>().Update(entity);
        }
    }
}
