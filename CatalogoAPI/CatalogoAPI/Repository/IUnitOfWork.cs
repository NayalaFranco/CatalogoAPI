namespace CatalogoAPI.Repository
{
    public interface IUnitOfWork
    {
        // Une os repositórios
        IProdutoRepository ProdutoRepository { get; }
        ICategoriaRepository CategoriaRepository { get; }
        // Adiciona o commit
        void Commit();
    }
}
