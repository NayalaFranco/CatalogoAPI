using AutoMapper;
using CatalogoAPI.DTOs;
using CatalogoAPI.Filters;
using CatalogoAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace CatalogoAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        // AutoMapper
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork context, IMapper mapper)
        {
            _uow = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        /* ActionResult retorna os metodos action como BadRequest e NotFound
         * E também o tipo que ele estiver definido.
         * * * * * *
         * Porque usar IEnumerable aqui?
         * 1- Porque essa interface é somente leitura.
         * 2- IEnumerable permite adiar a execução, ou seja, ele vai trabalhar por demanda.
         * 3- E usando IEnumerable não precisa ter toda a coleção na memória.
         * Daria para usar List mas IEnumerable AQUI é mais otimizado.
         */
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutos()
        {
            // Agora com o padrão repository usa o ProdutoRepository.Get() para essa operação
            var produtos = _uow.ProdutoRepository.Get().ToList();
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados...");
            }
            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);
            return Ok(produtosDto);
        }

        [HttpGet("menorpreco")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosPrecos()
        {
            var produtos = _uow.ProdutoRepository.GetProdutosPorPreco().ToList();
            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDto;

        }

        // Define que vai receber um id, e restringe a ser um inteiro.
        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<ProdutoDTO> GetProdutoId(int id)
        {
            var produto = _uow.ProdutoRepository.GetById(p => p.ProdutoId == id);
            if (produto is null)
            {
                return NotFound($"Produto com id= {id} não localizado...");
            }
            var produtoDto = _mapper.Map<ProdutoDTO>(produto);
            return Ok(produtoDto);

        }

        [HttpPost]
        public ActionResult PostProduto(ProdutoDTO produtoDto)
        {
            if (produtoDto is null)
                return BadRequest();

            // Inverte o mapeamento aqui
            var produto = _mapper.Map<Produto>(produtoDto);

            _uow.ProdutoRepository.Add(produto);
            _uow.Commit();

            //produtoDto = _mapper.Map<ProdutoDTO>(produto);

            // Similar ao CreatedAtAction mas informa uma rota para o nome
            // definido na action get ao invés do nome da action,
            // necessário aqui já que não estamos dando nomes especificos
            // para as actions
            return new CreatedAtRouteResult("ObterProduto",
                new { id = produto.ProdutoId }, produtoDto);

        }


        // Put = Atualização COMPLETA do produto (não permite parcial)
        [HttpPut("{id:int}")]
        public ActionResult PutProduto(int id, ProdutoDTO produtoDto)
        {
            // Quando enviar os dados do produto tem que
            // informar o id do produto também.
            // Então torna-se necessario conferir os id.
            if (id != produtoDto.ProdutoId)
            {
                return BadRequest($"O id informado ({id}) não é o mesmo id recebido para atualização ({produtoDto.ProdutoId})");
            }

            var produto = _mapper.Map<Produto>(produtoDto);

            _uow.ProdutoRepository.Update(produto);
            _uow.Commit();

            return Ok(produtoDto);


        }

        [HttpDelete("{id:int}")]
        public ActionResult DeleteProduto(int id)
        { 
            var produto = _uow.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound($"Produto com id= {id} não localizado...");

            _uow.ProdutoRepository.Delete(produto);
            _uow.Commit();

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDto);
        }










        /*
        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        /* ActionResult retorna os metodos action como BadRequest e NotFound
         * E também o tipo que ele estiver definido.
         * * * * * *
         * Porque usar IEnumerable aqui?
         * 1- Porque essa interface é somente leitura.
         * 2- IEnumerable permite adiar a execução, ou seja, ele vai trabalhar por demanda.
         * 3- E usando IEnumerable não precisa ter toda a coleção na memória.
         * Daria para usar List mas IEnumerable AQUI é mais otimizado.
         * * * * * *
         * async torna o metodo assíncrono, Task é necessario nesse cenario assíncrono
         */
        /*
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutosAsync()
        {
            // AsNoTracking melhora a performance mas só deve ser usado em Gets
            // Take limita a quantidade de resultados para não sobrecarregar o sistema.
            // await aguarda a resposta do servidor em um metodo async,
            // ToListAsync() é necessario em metodos async.
            var produtos = await _context.Produtos.AsNoTracking().Take(10).ToListAsync();
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados...");
            }
            return Ok(produtos);
        }

        // Define que vai receber um id, e restringe a ser um inteiro.
        [HttpGet("{id:int}", Name = "ObterProduto")]
        public async Task<ActionResult<Produto>> GetProdutoIdAsync(int id)
        {
            // First busca e retorna o primeiro resultado compativel, senao ele retorna uma excessão.
            // FirstOrDefault retorna o primeiro resultado compativel, senao ele retorna um null.
            var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.ProdutoId == id);
            if (produto is null)
            {
                return NotFound($"Produto com id= {id} não localizado...");
            }
            return Ok(produto);

        }

        [HttpPost]
        public async Task<ActionResult> PostProdutoAsync(Produto produto)
        {
            if (produto is null)
                return BadRequest();

            // Como o context está na memoria não é necessario
            // que o Add seja AddAsync, apenas o SaveChanges 
            // pois é ele que vai salvar no banco de dados.
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            // Similar ao CreatedAtAction mas informa uma rota para o nome
            // definido na action get ao invés do nome da action,
            // necessário aqui já que não estamos dando nomes especificos
            // para as actions
            return new CreatedAtRouteResult("ObterProduto",
                new { id = produto.ProdutoId }, produto);

        }


        // Put = Atualização COMPLETA do produto (não permite parcial)
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutProdutoAsync(int id, Produto produto)
        {
            // Quando enviar os dados do produto tem que
            // informar o id do produto também.
            // Então torna-se necessario conferir os id.
            if (id != produto.ProdutoId)
            {
                return BadRequest($"O id informado ({id}) não é o mesmo id recebido para atualização ({produto.ProdutoId})");
            }

            // Como estamos trabalhando em um cenario "desconectado"
            // (os dados estão dentro da variavel _context)
            // o contexto precisa ser informado que produto está em um
            // estado modificado. Para isso usamos o metodo Entry do contexto.
            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(produto);


        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProdutoAsync(int id)
        {
            // await somente onde tem que aguardar uma operação externa.
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound($"Produto com id= {id} não localizado...");

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return Ok(produto);

        }
        */
    }
}
