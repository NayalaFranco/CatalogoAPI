namespace Models;

public class Categoria
{
    // ao colocar uma propriedade int com o nome de Id
    // ou AlgumNomeId o EF automaticamente reconhece
    // como uma chave primaria
    public int CategoriaId { get; set; }
    public string? Nome { get; set; }
    public string? ImagemUrl { get; set; }
}
