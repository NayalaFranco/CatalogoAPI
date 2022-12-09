using CatalogoAPI.Validations;
using Microsoft.OpenApi.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models;
// IValidatableObject serve para criar validação de propriedades a nivel de Model
public class Produto : IValidatableObject
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


    // Sistema de validação a nivel de Model
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        //if (!string.IsNullOrEmpty(this.Nome))
        //{
        //    var primeiraLetra = this.Nome[0].ToString();
        //    if (primeiraLetra != primeiraLetra.ToUpper())
        //    {
        //        yield return new
        //          ValidationResult("Aprimeira letra do produto deve ser maiúscula",
        //          new[]
        //          { nameof(this.Nome) }
        //          );
        //    }
        //}

        if (this.Estoque <= 0)
        {
            yield return new
                ValidationResult("O estoque deve ser maior que zero",
                new[]
                { nameof(this.Estoque) }
                );
        }

        // exemplo de interação de propriedades que
        // essa forma de validação permite e a via Atributo não
        // "Easter Egg"
        if (this.Nome == "Café" && this.Estoque == 0)
        {
            yield return new
                ValidationResult("CAFÉÉÉÉ!!! QUERO CAFÉÉÉ!!! NÃO TEM CAFÉÉÉ!!!!!!!!!",
                new[]
                { "Easter Egg" });
        }

    }

}

