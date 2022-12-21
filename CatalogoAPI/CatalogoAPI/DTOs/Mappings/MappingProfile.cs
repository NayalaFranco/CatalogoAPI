using AutoMapper;
using Models;

namespace CatalogoAPI.DTOs.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Cria um mapeamento entre a entidade e o DTO
            // o reverseMap é para que esse mapeamento também 
            // seja "vice versa".
            CreateMap<Produto, ProdutoDTO>().ReverseMap();
            CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        }        
    }
}
