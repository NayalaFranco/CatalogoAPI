using CatalogoAPI.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace CatalogoAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly CatalogoAPIContext _context;

        public CategoriasController(CatalogoAPIContext context)
        {
            _context = context;
        }

        // Detalhes de Async/Task/Await na classe ProdutosController.

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoriasAsync()
        {
            try
            {
                //throw new DataMisalignedException();

                // AsNoTracking melhora a performance mas só deve ser usado em Gets
                // Take limita a quantidade de resultados para não sobrecarregar o sistema.
                var categorias = await _context.Categorias.AsNoTracking().Take(10).ToListAsync();
                if (categorias is null)
                    return NotFound("Categorias não encontradas...");

                return Ok(categorias);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação");
            }
            
        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoriasProdutosAsync()
        {
            try
            {
                // Recomendado nunca retornar objetos relacionados sem um filtro,
                // aqui está usando Where para retornar somente os id menor ou igual a 10.
                var categorias = await _context.Categorias.Include(p => p.Produtos)
                    .AsNoTracking().Where(c => c.CategoriaId <= 10).ToListAsync();
                if (categorias is null)
                    return NotFound("Categorias não encontradas...");

                return Ok(categorias);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação");
            }
            
            
        }

        // Define que vai receber um id, e restringe a ser um inteiro.
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<Categoria>> GetCategoriasIdAsync(int id)
        {
            try
            {
                // First busca e retorna o primeiro resultado compativel, senao ele retorna uma excessão.
                // FirstOrDefault retorna o primeiro resultado compativel, senao ele retorna um null.
                var categoria = await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.CategoriaId == id);
                if (categoria is null)
                {
                    return NotFound($"Categoria com id= {id} não localizada...");
                }
                return Ok(categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação");
            }
            
        }

        [HttpPost]
        public async Task<ActionResult> PostCategoriaAsync(Categoria categoria)
        {
            try
            {
                if (categoria is null)
                    return BadRequest();

                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();

                // Similar ao CreatedAtAction mas informa uma rota para o nome
                // definido na action get ao invés do nome da action,
                // necessário aqui já que não estamos dando nomes especificos
                // para as actions
                return new CreatedAtRouteResult("ObterCategoria",
                    new { id = categoria.CategoriaId }, categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação");
            }
            
        }


        // Put = Atualização COMPLETA do categoria (não permite parcial)
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutCategoriaAsync(int id, Categoria categoria)
        {
            try
            {
                // Quando enviar os dados do categoria tem que
                // informar o id do categoria também.
                // Então torna-se necessario conferir os id.
                if (id != categoria.CategoriaId)
                {
                    return BadRequest($"O id informado ({id}) não é o mesmo id recebido para atualização ({categoria.CategoriaId})");
                }

                // Como estamos trabalhando em um cenario "desconectado"
                // (os dados estão dentro da variavel _context)
                // o contexto precisa ser informado que categoria está em um
                // estado modificado. Para isso usamos o metodo Entry do contexto.
                _context.Entry(categoria).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação");
            }
            
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteCategoriaAsync(int id)
        {
            try
            {
                var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == id);

                if (categoria is null)
                    return NotFound($"Categoria com id= {id} não localizada...");

                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();

                return Ok(categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação");
            }
            
        }


    }
}
