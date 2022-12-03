using CatalogoAPI.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace CatalogoAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly CatalogoAPIContext _context;

        public ProdutosController(CatalogoAPIContext context)
        {
            _context = context;
        }


        [HttpGet]
        /* ActionResult retorna os metodos action como BadRequest e NotFound
         * E também o tipo que ele estiver definido.
         * * * * * *
         * Porque usar IEnumerable aqui?
         * 1- Porque essa interface é somente leitura.
         * 2- IEnumerable permite adiar a execução, ou seja, ele vai trabalhar por demanda.
         * 3- E usando IEnumerable não precisa ter toda a coleção na memória.
         * Daria para usar List mas IEnumerable AQUI é mais otimizado.
         */
        public ActionResult<IEnumerable<Produto>> Get()
        {
            try
            {
                // AsNoTracking melhora a performance mas só deve ser usado em Gets
                // Take limita a quantidade de resultados para não sobrecarregar o sistema.
                var produtos = _context.Produtos.AsNoTracking().Take(10).ToList();
                if (produtos is null)
                {
                    return NotFound("Produtos não encontrados...");
                }
                return Ok(produtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação");
            }
        }

        // Define que vai receber um id, e restringe a ser um inteiro.
        [HttpGet("{id:int}", Name ="ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            try
            {
                // First busca e retorna o primeiro resultado compativel, senao ele retorna uma excessão.
                // FirstOrDefault retorna o primeiro resultado compativel, senao ele retorna um null.
                var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);
                if (produto is null)
                {
                    return NotFound("Produto não encontrado...");
                }
                return Ok(produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                    "Ocorreu um problema ao tratar a sua solicitação");
            }
            
        }

        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            try
            {
                if (produto is null)
                    return BadRequest();

                _context.Produtos.Add(produto);
                _context.SaveChanges();

                // Similar ao CreatedAtAction mas informa uma rota para o nome
                // definido na action get ao invés do nome da action,
                // necessário aqui já que não estamos dando nomes especificos
                // para as actions
                return new CreatedAtRouteResult("ObterProduto",
                    new { id = produto.ProdutoId }, produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                    "Ocorreu um problema ao tratar a sua solicitação");
            }
            
        }


        // Put = Atualização COMPLETA do produto (não permite parcial)
        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            try
            {
                // Quando enviar os dados do produto tem que
                // informar o id do produto também.
                // Então torna-se necessario conferir os id.
                if (id != produto.ProdutoId)
                {
                    return BadRequest();
                }

                // Como estamos trabalhando em um cenario "desconectado"
                // (os dados estão dentro da variavel _context)
                // o contexto precisa ser informado que produto está em um
                // estado modificado. Para isso usamos o metodo Entry do contexto.
                _context.Entry(produto).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                    "Ocorreu um problema ao tratar a sua solicitação");
            }
            
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

                if (produto is null)
                    return NotFound("Produto não localizado...");

                _context.Produtos.Remove(produto);
                _context.SaveChanges();

                return Ok(produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                    "Ocorreu um problema ao tratar a sua solicitação");
            }
            
        }

    }
}
