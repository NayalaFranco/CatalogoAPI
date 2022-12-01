using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoAPI.Migrations
{
    /// <inheritdoc />
    public partial class PopulaProdutos : Migration
    {
        /// <inheritdoc />
        /// Popula a tabela produtos
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId)" +
                "VALUES('Coca-Cola Zero', 'Refrigerante de Cola 350 ml', 5.45, 'cocacolazero.jpg', 50, GETDATE(), 1)");

            mb.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId)" +
                "VALUES('Hamburguer 300', 'Hamburguer com 2 hamburgueres de 150g, Queijo Mussarela, Tomate, Alface e Maionese', 32.90, 'hamburguer300.jpg', 25, GETDATE(), 2)");

            mb.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId)" +
                "VALUES('Sorvete de Amarula', 'Bola de Sorvete de Amarula', 3.50, 'sovetebola.jpg', 45, GETDATE(), 3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("DELETE FROM Produtos");
        }
    }
}
