using CatalogoAPI.Context;
using Models;

namespace CatalogoAPIxUnitTests
{
    internal class DBUnitTestsMockInitializer
    {
        // Usaria se fosse usar um mock
        public DBUnitTestsMockInitializer()
        {}
        
        // Popula o banco com dados
        public void Seed(CatalogoAPIContext context)
        {
            context.Categorias.Add
            (new Categoria { CategoriaId = 777, Nome = "Bebidas777", ImagemUrl = "bebidas777.jpg" });

            context.Categorias.Add
            (new Categoria { CategoriaId = 1, Nome = "Bebidas", ImagemUrl = "bebidas.jpg" });

            context.Categorias.Add
            (new Categoria { CategoriaId = 2, Nome = "Lanches", ImagemUrl = "lanches.jpg" });

            context.Categorias.Add
            (new Categoria { CategoriaId = 3, Nome = "Sobremesas", ImagemUrl = "sobremesas.jpg" });

            context.Categorias.Add
            (new Categoria { CategoriaId = 4, Nome = "Sucos", ImagemUrl = "sucos1.jpg" });

            context.Categorias.Add
            (new Categoria { CategoriaId = 5, Nome = "Doces", ImagemUrl = "doces1.jpg" });

            context.Categorias.Add
            (new Categoria { CategoriaId = 6, Nome = "Salgados", ImagemUrl = "Salgados1.jpg" });

            context.Categorias.Add
            (new Categoria { CategoriaId = 7, Nome = "Tortas", ImagemUrl = "tortas1.jpg" });

            context.Categorias.Add
            (new Categoria { CategoriaId = 8, Nome = "Bolos", ImagemUrl = "bolos1.jpg" });

            

            context.SaveChanges();
        }
    }
}
