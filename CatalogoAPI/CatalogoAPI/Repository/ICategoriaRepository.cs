using CatalogoAPI.Pagination;
using Models;

namespace CatalogoAPI.Repository
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriasParameters);
        Task<PagedList<Categoria>> GetCategoriasProdutos(CategoriasParameters categoriasParameters);
    }
}
