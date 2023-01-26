using CatalogoAPI.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CatalogoAPI.Repository
{
    // Esse "where T : class" é para especificar que é para receber
    // somente tipos que são classes, exemplo Produto.
    public class Repository<T> : IRepository<T> where T : class
    {
        // É preciso fazer a injeção de dependência do DBContext
        // do EF aqui
        protected CatalogoAPIContext _context;

        public Repository(CatalogoAPIContext context)
        {
            _context = context;
        }

        // Então começamos a implementar os métodos
        public IQueryable<T> Get()
        {
            return _context.Set<T>().AsNoTracking();
        }

        // O predicate é um delegate que verifica se o parâmetro
        // atende a uma condição especifica ou não.
        public async Task<T> GetById(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(predicate);
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
            // Não precisa exatamente desse entity state, é mais para reforçar
            // pois ele ja esta sendo rastreado e o EF ja vai saber que
            // ele está modificado, mas reforçando garante melhor que sabe.
            _context.Entry(entity).State = EntityState.Modified;
            _context.Set<T>().Update(entity);
        }
    }
}
