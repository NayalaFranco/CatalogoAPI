using CatalogoAPI.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models;
public class Produto
{
    // ao colocar uma propriedade int com o nome de Id
    // ou AlgumNomeId o EF automaticamente reconhece
    // como uma chave primaria.
    // Obs: o Id tem que ser depois do nome, antes o EF n reconhece.
    public int ProdutoId { get; set; }

    [Required(ErrorMessage = "O nome do produto é obrigatório!")]
    [StringLength(100, MinimumLength =2, ErrorMessage = "O nome deve ter no mínimo 2 e no máximo 100 caracteres.")]
    // Meu atributo personalizado
    [PrimeiraLetraMaiuscula]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória!")]
    [StringLength(300, MinimumLength = 5, ErrorMessage = "A descrição deve ter no mínimo 5 e no máximo 300 caracteres.")]
    public string? Descricao { get; set; }

    // DataAnnotation para definir a precisão do tipo decimal no DB
    // e não aparecer mensagem amarela no log ao criar o migrations
    [Column(TypeName = "decimal(10,4)")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "Uma URL de imagem é obrigatória!")]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }

    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }


    // Melhora a definição de qual categoria está vinculado a FK
    public int CategoriaId { get; set; }

    // Ignora a propriedade durante a serialização e desserialização
    [JsonIgnore]
    public Categoria? Categoria { get; set; }
}
