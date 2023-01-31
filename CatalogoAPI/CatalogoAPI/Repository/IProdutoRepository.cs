using CatalogoAPI.Pagination;
using Models;

namespace CatalogoAPI.Repository
{
    // Interface especifica para implementar os métodos específicos
    // ou se não houver os métodos específicos, precisa para deixar 
    // definido o tipo que neste caso é <Produto>
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<IEnumerable<Produto>> GetProdutosPorPreco();
        Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters);
    }
}
