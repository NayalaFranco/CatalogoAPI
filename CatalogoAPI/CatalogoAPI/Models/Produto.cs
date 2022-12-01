using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;
public class Produto
{
    // ao colocar uma propriedade int com o nome de Id
    // ou AlgumNomeId o EF automaticamente reconhece
    // como uma chave primaria.
    // Obs: o Id tem que ser depois do nome, antes o EF n reconhece.
    public int ProdutoId { get; set; }

    [Required]
    [StringLength(100)]
    public string? Nome { get; set; }

    [Required]
    [StringLength(300)]
    public string? Descricao { get; set; }

    // DataAnnotation para definir a precisão do tipo decimal no DB
    // e não aparecer mensagem amarela no log ao criar o migrations
    [Column(TypeName = "decimal(10,4)")]
    public decimal Preco { get; set; }

    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }

    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }


    // Melhora a definição de qual categoria está vinculado a FK
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}
