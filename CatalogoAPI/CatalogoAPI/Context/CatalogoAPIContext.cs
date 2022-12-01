using Microsoft.EntityFrameworkCore;
using Models;

namespace CatalogoAPI.Context;

public class CatalogoAPIContext : DbContext
{
    public CatalogoAPIContext(DbContextOptions<CatalogoAPIContext> options) : base(options)
    {

    }

    public DbSet<Categoria>? Categorias { get; set; }
    public DbSet<Produto>? Produtos { get; set; }
}
