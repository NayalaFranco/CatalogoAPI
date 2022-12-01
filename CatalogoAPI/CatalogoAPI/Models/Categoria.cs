using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Models;

public class Categoria
{
    // ao colocar uma propriedade int com o nome de Id
    // ou AlgumNomeId o EF automaticamente reconhece
    // como uma chave primaria.
    // Obs: o Id tem que ser depois do nome, antes o EF n reconhece.
    public int CategoriaId { get; set; }
    [Required]
    [StringLength(100)]
    public string? Nome { get; set; }
    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }

    // Convenção EF de Um para Muitos
    // Só isso ja seria o suficiente, mas da para
    // explicitar em produtos um vinculo a essa categoria
    // para melhorar.
    public ICollection<Produto>? Produtos { get; set; }


    // Boa pratica que a classe que usa a coleção inicialize a coleção
    public Categoria()
    {
        Produtos = new Collection<Produto>();
    }
}
