﻿using AutoMapper;
using CatalogoAPI.DTOs;
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

    [Route("api/[controller]")]
    [ApiController]
    // Faz a controladora exigir autenticação do tipo bearer para os requests.
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork context, ILogger<CategoriasController> logger, IMapper mapper)
        {
            _uow = context;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtem as categorias.
        /// </summary>
        /// <param name="categoriasParameters">Objeto com os dados de paginação</param>
        /// <returns>Retorna uma lista de categorias</returns>
        [HttpGet]
        [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>>
            GetCategorias([FromQuery] CategoriasParameters categoriasParameters)
        {
            try
            {
                //throw new Exception();

                // isso será logado no console
                _logger.LogInformation("========== GET api/categorias =============");

                var categorias = await _uow.CategoriaRepository.GetCategorias(categoriasParameters);
                if (categorias is null)
                    return NotFound("Categorias não encontradas...");

                // Cria um metadata adicionar os dados de paginação no header do response
                var metadata = new
                {
                    categorias.TotalCount,
                    categorias.PageSize,
                    categorias.CurrentPage,
                    categorias.TotalPages,
                    categorias.HasNext,
                    categorias.HasPrevious
                };

                // Serializa em Json o metadata e adiciona no Header do Response.
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

                var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

                return Ok(categoriasDto);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Obtem uma lista de categorias com os produtos contidos nelas.
        /// </summary>
        /// <param name="categoriasParameters">Objeto com os dados de paginação</param>
        /// <returns>Retorna uma lista de categorias com os produtos</returns>
        [HttpGet("produtos")]
        [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> 
            GetCategoriasProdutos([FromQuery] CategoriasParameters categoriasParameters)
        {
            
            var categorias = await _uow.CategoriaRepository.GetCategoriasProdutos(categoriasParameters);
            if (categorias is null)
                return NotFound("Categorias não encontradas...");

            // Cria um metadata adicionar os dados de paginação no header do response
            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };

            // Serializa em Json o metadata e adiciona no Header do Response.
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

            return Ok(categoriasDto);
        }

        /// <summary>
        /// Obtem uma unica categoria pelo seu ID
        /// </summary>
        /// <param name="id">ID da categoria</param>
        /// <returns>Retorna o objeto categoria de mesmo ID</returns>
        // Define que vai receber um id, e restringe a ser um inteiro.
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CategoriaDTO>> GetCategoriasId(int id)
        {

            var categoria = await _uow.CategoriaRepository.GetById(c => c.CategoriaId == id);

            _logger.LogInformation($"========== GET api/categorias/id = {id} =============");

            if (categoria is null)
            {
                _logger.LogInformation($"========== GET api/categorias/id = {id} NOT FOUND =============");
                return NotFound($"Categoria com id= {id} não localizada...");
            }

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
            return Ok(categoriaDto);
        }

        /// <summary>
        /// Regista uma nova categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///     POST: Categorias
        ///     
        ///     {
        ///         "categoriaId": 1,
        ///         "nome": "categoria1",
        ///         "imagemUrl": "http://teste.net/1.jpg"
        ///     }     
        /// </remarks>
        /// <param name="categoriaDto">Objeto Categoria</param>
        /// <returns>O objeto categoria criado</returns>
        /// <remarks>Retorna o objeto categoria que foi criado</remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> PostCategoria(CategoriaDTO categoriaDto)
        {
            if (categoriaDto is null)
                return BadRequest();

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _uow.CategoriaRepository.Add(categoria);
            await _uow.Commit();

            // Similar ao CreatedAtAction mas informa uma rota para o nome
            // definido na action get ao invés do nome da action,
            // necessário aqui já que não estamos dando nomes específicos
            // para as actions
            return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoriaDto.CategoriaId }, categoriaDto);
        }



        /// <summary>
        /// Atualiza os dados de uma categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///     PUT: Categorias/1
        ///     
        ///     {
        ///         "categoriaId": 1,
        ///         "nome": "categoria1",
        ///         "imagemUrl": "http://teste.net/1.jpg"
        ///     }     
        /// </remarks>
        /// <param name="id">ID da Categoria</param>
        /// <param name="categoriaDto">Objeto Categoria</param>
        /// <returns>Retorna a categoria atualizada</returns>
        // Put = Atualização COMPLETA do categoria (não permite parcial)
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> PutCategoria(int id, CategoriaDTO categoriaDto)
        {
            // Quando enviar os dados do categoria tem que
            // informar o id do categoria também.
            // Então torna-se necessário conferir os id.
            if (id != categoriaDto.CategoriaId)
            {
                return BadRequest($"O id informado ({id}) não é o mesmo id recebido para atualização ({categoriaDto.CategoriaId})");
            }

            // Confere se essa categoria existe mesmo na DB
            var categoria = await _uow.CategoriaRepository.GetById(p => p.CategoriaId == id);
            if (categoria is null)
            {
                return NotFound($"Categoria com id= {id} não localizada...");
            }

            categoria = _mapper.Map<Categoria>(categoriaDto);

            _uow.CategoriaRepository.Update(categoria);
            await _uow.Commit();

            return Ok();
        }


        /// <summary>
        /// Deleta a categoria de mesmo ID
        /// </summary>
        /// <param name="id">Identificação da categoria</param>
        /// <returns>Confirmação e a categoria deletada</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CategoriaDTO>> DeleteCategoria(int id)
        {
            var categoria = await _uow.CategoriaRepository.GetById(c => c.CategoriaId == id);

            if (categoria is null)
                return NotFound($"Categoria com id= {id} não localizada...");

            _uow.CategoriaRepository.Delete(categoria);
            await _uow.Commit();

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(categoriaDto);
        }

        /*

        // Detalhes de Async/Task/Await na classe ProdutosController.

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoriasAsync()
        {
            //throw new Exception();

            // isso será logado no console
            _logger.LogInformation("========== GET api/categorias =============");

            // AsNoTracking melhora a performance mas só deve ser usado em Gets
            // Take limita a quantidade de resultados para não sobrecarregar o sistema.
            var categorias = await _context.Categorias.AsNoTracking().Take(10).ToListAsync();
            if (categorias is null)
                return NotFound("Categorias não encontradas...");

            return Ok(categorias);
        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoriasProdutosAsync()
        {
            // Recomendado nunca retornar objetos relacionados sem um filtro,
            // aqui está usando Where para retornar somente os id menor ou igual a 10.
            var categorias = await _context.Categorias.Include(p => p.Produtos)
                .AsNoTracking().Where(c => c.CategoriaId <= 10).ToListAsync();
            if (categorias is null)
                return NotFound("Categorias não encontradas...");

            return Ok(categorias);
        }

        // Define que vai receber um id, e restringe a ser um inteiro.
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<Categoria>> GetCategoriasIdAsync(int id)
        {
            // First busca e retorna o primeiro resultado compatível, senão ele retorna uma exceção.
            // FirstOrDefault retorna o primeiro resultado compatível, senão ele retorna um null.
            var categoria = await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.CategoriaId == id);

            _logger.LogInformation($"========== GET api/categorias/id = {id} =============");

            if (categoria is null)
            {
                _logger.LogInformation($"========== GET api/categorias/id = {id} NOT FOUND =============");
                return NotFound($"Categoria com id= {id} não localizada...");
            }
            return Ok(categoria);

        }

        [HttpPost]
        public async Task<ActionResult> PostCategoriaAsync(Categoria categoria)
        {
            if (categoria is null)
                return BadRequest();

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            // Similar ao CreatedAtAction mas informa uma rota para o nome
            // definido na action get ao invés do nome da action,
            // necessário aqui já que não estamos dando nomes específicos
            // para as actions
            return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoria.CategoriaId }, categoria);
        }


        // Put = Atualização COMPLETA do categoria (não permite parcial)
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutCategoriaAsync(int id, Categoria categoria)
        {
            // Quando enviar os dados do categoria tem que
            // informar o id do categoria também.
            // Então torna-se necessário conferir os id.
            if (id != categoria.CategoriaId)
            {
                return BadRequest($"O id informado ({id}) não é o mesmo id recebido para atualização ({categoria.CategoriaId})");
            }

            // Como estamos trabalhando em um cenário "desconectado"
            // (os dados estão dentro da variável _context)
            // o contexto precisa ser informado que categoria está em um
            // estado modificado. Para isso usamos o método Entry do contexto.
            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteCategoriaAsync(int id)
        {
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == id);

            if (categoria is null)
                return NotFound($"Categoria com id= {id} não localizada...");

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return Ok(categoria);
        }

        */
    }
}
