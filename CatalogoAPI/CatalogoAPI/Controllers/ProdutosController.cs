using AutoMapper;
using CatalogoAPI.DTOs;
using CatalogoAPI.Filters;
using CatalogoAPI.Pagination;
using CatalogoAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Text.Json;

namespace CatalogoAPI.Controllers
{
    // Esse Produces indica que as saidas de objetos serão no formato JSON
    // seu oposto é o Consumes que indica que a API só aceita entradas no formato JSON
    // qualquer outro formato produz um erro: "415 - Unsupported Media Type".
    // Neste momento estamos usando apenas para deixar os gets do swagger sempre
    // com a opção de mostrar no formato application/json selecionada.
    [Produces("application/json")]

    [Route("[controller]")]
    [ApiController]
    // Faz a controladora exigir autenticação do tipo bearer para os requests.
    [Authorize(AuthenticationSchemes = "Bearer")]
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


        /// <summary>
        /// Obtem os produtos
        /// </summary>
        /// <param name="produtosParameters">Objeto com os dados de paginação</param>
        /// <returns>Lista de produtos</returns>
        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        /* ActionResult retorna os métodos action como BadRequest e NotFound
         * E também o tipo que ele estiver definido.
         * * * * * *
         * Porque usar IEnumerable aqui?
         * 1- Porque essa interface é somente leitura.
         * 2- IEnumerable permite adiar a execução, ou seja, ele vai trabalhar por demanda.
         * 3- E usando IEnumerable não precisa ter toda a coleção na memória.
         * Daria para usar List mas IEnumerable AQUI é mais otimizado.
         */
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            // Agora com o padrão repository usa o ProdutoRepository.Get() para essa operação
            var produtos = await _uow.ProdutoRepository.GetProdutos(produtosParameters);
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados...");
            }

            // Cria um metadata adicionar os dados de paginação no header do response
            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious
            };

            // Serializa em Json o metadata e adiciona no Header do Response.
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));


            // Converte para lista de DTO
            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);
            // Entrega o DTO
            return Ok(produtosDto);
        }

        /// <summary>
        /// Obtem os produtos ordenados pelo menor preço
        /// </summary>
        /// <returns>Lista de produtos</returns>
        [HttpGet("menorpreco")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPrecos()
        {
            var produtos = await _uow.ProdutoRepository.GetProdutosPorPreco();
            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDto;

        }

        /// <summary>
        /// Obtem um produto pelo seu ID
        /// </summary>
        /// <param name="id">Número de identificação do produto</param>
        /// <returns>Retorna um objeto produto</returns>
        // Define que vai receber um id, e restringe a ser um inteiro.
        [HttpGet("{id:int}", Name = "ObterProduto")]
        public async Task<ActionResult<ProdutoDTO>> GetProdutoId(int id)
        {
            var produto = await _uow.ProdutoRepository.GetById(p => p.ProdutoId == id);
            if (produto is null)
            {
                return NotFound($"Produto com id= {id} não localizado...");
            }
            // Converte para DTO
            var produtoDto = _mapper.Map<ProdutoDTO>(produto);
            return Ok(produtoDto);

        }

        /// <summary>
        /// Cria um novo produto
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///     POST: Produtos
        ///     
        ///     {
        ///         "produtoId": 1,
        ///         "nome": "produto1",
        ///         "descricao": "um produto muito bom",
        ///         "preco": 12.34,
        ///         "imagemUrl": "http://teste.net/1.jpg",
        ///         "categoriaID: 1
        ///     }   
        /// </remarks>
        /// <param name="produtoDto">Objeto ProdutoDTO</param>
        /// <returns>Retorna o objeto do produto criado</returns>
        [HttpPost]
        public async Task<ActionResult> PostProduto(ProdutoDTO produtoDto)
        {
            if (produtoDto is null)
                return BadRequest();

            // Inverte o mapeamento aqui
            // Converte ProdutoDTO para Produto
            var produto = _mapper.Map<Produto>(produtoDto);

            _uow.ProdutoRepository.Add(produto);
            await _uow.Commit();

            //produtoDto = _mapper.Map<ProdutoDTO>(produto);

            // Similar ao CreatedAtAction mas informa uma rota para o nome
            // definido na action get ao invés do nome da action,
            // necessário aqui já que não estamos dando nomes específicos
            // para as actions
            return new CreatedAtRouteResult("ObterProduto",
                // Mas mostra produtoDto
                new { id = produto.ProdutoId }, produtoDto);

        }

        /// <summary>
        /// Atualiza os dados de um produto
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///     PUT: Produtos/1
        ///     
        ///     {
        ///         "produtoId": 1,
        ///         "nome": "produto1",
        ///         "descricao": "um produto muito bom",
        ///         "preco": 12.34,
        ///         "imagemUrl": "http://teste.net/1.jpg",
        ///         "categoriaID: 1
        ///     }   
        /// </remarks>
        /// <param name="id">ID do Produto</param>
        /// <param name="produtoDto">Objeto ProdutoDTO</param>
        /// <returns>Confirma o sucesso e retorna o objeto do produto atualizado</returns>
        // Put = Atualização COMPLETA do produto (não permite parcial)
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutProduto(int id, ProdutoDTO produtoDto)
        {
            // Quando enviar os dados do produto tem que
            // informar o id do produto também.
            // Então torna-se necessário conferir os id.
            if (id != produtoDto.ProdutoId)
            {
                return BadRequest($"O id informado ({id}) não é o mesmo id recebido para atualização ({produtoDto.ProdutoId})");
            }

            // Verifica se esse produto a ser atualizado existe mesmo.
            var produto = await _uow.ProdutoRepository.GetById(p => p.ProdutoId == id);
            if (produto is null)
            {
                return NotFound($"Produto com id= {id} não localizado...");
            }

            // Converte ProdutoDTO para Produto
            produto = _mapper.Map<Produto>(produtoDto);

            _uow.ProdutoRepository.Update(produto);
            await _uow.Commit();

            // Mas mostra produtoDto
            return Ok(produtoDto);
        }

        /// <summary>
        /// Deleta um produto
        /// </summary>
        /// <param name="id">Código do produto</param>
        /// <returns>Confirma o sucesso e retorna o objeto do produto deletado</returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduto(int id)
        { 
            var produto = await _uow.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound($"Produto com id= {id} não localizado...");

            _uow.ProdutoRepository.Delete(produto);
            await _uow.Commit();

            // Converte ProdutoDTO para Produto
            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            // Mas mostra produtoDto
            return Ok(produtoDto);
        }










        /*
        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        /* ActionResult retorna os métodos action como BadRequest e NotFound
         * E também o tipo que ele estiver definido.
         * * * * * *
         * Porque usar IEnumerable aqui?
         * 1- Porque essa interface é somente leitura.
         * 2- IEnumerable permite adiar a execução, ou seja, ele vai trabalhar por demanda.
         * 3- E usando IEnumerable não precisa ter toda a coleção na memória.
         * Daria para usar List mas IEnumerable AQUI é mais otimizado.
         * * * * * *
         * async torna o método assíncrono, Task é necessário nesse cenário assíncrono
         */
        /*
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutosAsync()
        {
            // AsNoTracking melhora a performance mas só deve ser usado em Gets
            // Take limita a quantidade de resultados para não sobrecarregar o sistema.
            // await aguarda a resposta do servidor em um método async,
            // ToListAsync() é necessário em métodos async.
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
            // First busca e retorna o primeiro resultado compatível, senão ele retorna uma exceção.
            // FirstOrDefault retorna o primeiro resultado compatível, senão ele retorna um null.
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

            // Como o context está na memoria não é necessário
            // que o Add seja AddAsync, apenas o SaveChanges 
            // pois é ele que vai salvar no banco de dados.
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            // Similar ao CreatedAtAction mas informa uma rota para o nome
            // definido na action get ao invés do nome da action,
            // necessário aqui já que não estamos dando nomes específicos
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
            // Então torna-se necessário conferir os id.
            if (id != produto.ProdutoId)
            {
                return BadRequest($"O id informado ({id}) não é o mesmo id recebido para atualização ({produto.ProdutoId})");
            }

            // Como estamos trabalhando em um cenário "desconectado"
            // (os dados estão dentro da variável _context)
            // o contexto precisa ser informado que produto está em um
            // estado modificado. Para isso usamos o método Entry do contexto.
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
