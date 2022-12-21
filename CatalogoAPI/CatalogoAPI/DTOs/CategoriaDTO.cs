using Models;
using System.ComponentModel.DataAnnotations;

namespace CatalogoAPI.DTOs
{
    public class CategoriaDTO
    {
        // Na DTO não precisa por as DataAnnotations
        public int CategoriaId { get; set; }
        public string? Nome { get; set; }
        public string? ImagemUrl { get; set; }
        public ICollection<ProdutoDTO>? Produtos { get; set; }
    }
}
