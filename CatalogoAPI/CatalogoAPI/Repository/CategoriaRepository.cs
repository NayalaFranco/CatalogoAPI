using CatalogoAPI.Context;
using CatalogoAPI.Pagination;
using Microsoft.EntityFrameworkCore;
using Models;

namespace CatalogoAPI.Repository
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(CatalogoAPIContext context) : base(context)
        {
        }

        public async Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriasParameters)
        {
            return await PagedList<Categoria>.ToPagedList(Get().OrderBy(on => on.CategoriaId),
                categoriasParameters.PageNumber,
                categoriasParameters.PageSize);
        }

        public async Task<PagedList<Categoria>> GetCategoriasProdutos(CategoriasParameters categoriasParameters)
        {
            return await PagedList<Categoria>.ToPagedList(Get().OrderBy(on => on.CategoriaId).Include(x => x.Produtos),
                categoriasParameters.PageNumber,
                categoriasParameters.PageSize);
                ;
        }
    }
}
