using CatalogoAPI.Context;
using CatalogoAPI.Pagination;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Produto>> GetProdutosPorPreco()
        {
            return await Get().OrderBy(c => c.Preco).ToListAsync();
        }

        // Método de paginação
        public async Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters)
        {
            /*
            return Get()
                // Ordena por nome
                .OrderBy(n => n.Nome)
                // Pula os registros (efeito de mudar de pagina)
                .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
                // Pega a quantia de registros definido no PageSize
                .Take(produtosParameters.PageSize)
                // Joga os registros em uma lista.
                .ToList();
            */

            return await PagedList<Produto>.ToPagedList(Get().OrderBy(on => on.ProdutoId),
                produtosParameters.PageNumber, produtosParameters.PageSize);
        }
    }
}
