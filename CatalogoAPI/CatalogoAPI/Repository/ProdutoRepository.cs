using CatalogoAPI.Context;
using Models;

namespace CatalogoAPI.Repository
{
    // a classe de repositorio especifica implementa
    // a interface especifica e herda o repositorio genérico
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(CatalogoAPIContext context) : base(context)
        {
        }

        public IEnumerable<Produto> GetProdutosPorPreco()
        {
            return Get().OrderBy(c => c.Preco).ToList();
        }
    }
}
