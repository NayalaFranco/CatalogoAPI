using AutoMapper;
using CatalogoAPI.Context;
using CatalogoAPI.Controllers;
using CatalogoAPI.DTOs;
using CatalogoAPI.DTOs.Mappings;
using CatalogoAPI.Pagination;
using CatalogoAPI.Repository;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CatalogoAPIxUnitTests
{
    public class CategoriasUnitTestController
    {

        private IMapper mapper;
        private IUnitOfWork repository;

        public static DbContextOptions<CatalogoAPIContext> dbContextOption { get; }

        // Se quiser uma connection string diferente para realizar os testes
        // mas acho que o ideal seria colocar uma diferente no appsettings.development.json
        public static string connectionString =
            "Server=localhost\\sqlexpress; Initial Catalog=CatalogoApiDb; Integrated Security=True; Encrypt=True; TrustServerCertificate=True";


        // Construtores Estáticos são usados para inicializar dados estáticos
        // Ele é chamado automaticamente antes que a primeira instancia seja criada.
        static CategoriasUnitTestController()
        {
            dbContextOption = new DbContextOptionsBuilder<CatalogoAPIContext>()
                .UseSqlServer(connectionString)
                .Options;
        }

        public CategoriasUnitTestController()
        {
            // Config para o Auto Mapper
            var configMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            mapper = configMapper.CreateMapper();

            var context = new CatalogoAPIContext(dbContextOption);

            /* Caso estivesse usando um banco de dados novo e vazio habilitaria
             * isso e ele iria carregar o DB com o básico para os testes. */
            //DBUnitTestsMockInitializer db = new DBUnitTestsMockInitializer();
            //db.Seed(context);

            repository = new UnitOfWork(context);
        }

        // Testes Unitários

        /********************** TESTE GET ****************************/
        [Fact]
        // Boa prática o teste ter o nome do que está testando
        // e seu resultado esperado
        public async Task GetCategorias_Return_OkResult()
        {
            // Arrange
            var controller = new CategoriasController(repository, new NullLogger<CategoriasController>(), mapper);

            // Esse HttpContext é para parar de dar erro no código "Response.Headers.Add"
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };

            CategoriasParameters parameters = new CategoriasParameters()
            {
                PageNumber = 1,
                PageSize = 3
            };

            // Act
            var data = await controller.GetCategorias(parameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(data.Result);
            Assert.IsType<List<CategoriaDTO>>(okResult.Value);

            //Assert.IsType<ActionResult<IEnumerable<CategoriaDTO>>>(data);
        }

        [Fact]
        public async Task GetCategorias_Return_BadRequestResult()
        {
            // Arrange
            var controller = new CategoriasController(repository, new NullLogger<CategoriasController>(), mapper);

            // Act
            // Esse null vai forçar um nullexception que o trycatch vai pegar e devolver como badrequest
            var data = await controller.GetCategorias(null);

            // Assert
            Assert.IsType<BadRequestResult>(data.Result);
        }

        [Fact]
        public async Task GetCategorias_MatchResult()
        {
            // Arrange
            var controller = new CategoriasController(repository, new NullLogger<CategoriasController>(), mapper);

            // Esse HttpContext é para parar de dar erro no código "Response.Headers.Add"
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };

            CategoriasParameters parameters = new CategoriasParameters()
            { };

            // Act
            var data = await controller.GetCategorias(parameters);

            // Assert 
            var okResult = Assert.IsType<OkObjectResult>(data.Result);
            var cat = okResult.Value.Should().BeAssignableTo<List<CategoriaDTO>>().Subject;

            Assert.Equal("Bebidas", cat[0].Nome);
            Assert.Equal("bebidas.jpg", cat[0].ImagemUrl);

            Assert.Equal("Sobremesas", cat[2].Nome);
            Assert.Equal("sobremesas.jpg", cat[2].ImagemUrl);
        }

        /********************** TESTE GET BY ID **********************/
        [Fact]
        public async Task GetCategoriasId_Return_OkResult()
        {
            // Arrange
            var controller = new CategoriasController(repository, new NullLogger<CategoriasController>(), mapper);
            int catId = 2;

            // Act
            var data = await controller.GetCategoriasId(catId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(data.Result);
            Assert.IsType<CategoriaDTO>(okResult.Value);
        }

        [Fact]
        public async Task GetCategoriasId_Return_NotFound()
        {
            // Arrange
            var controller = new CategoriasController(repository, new NullLogger<CategoriasController>(), mapper);
            int catId = 9999999;

            // Act
            var data = await controller.GetCategoriasId(catId);

            // Assert
            var notFoundObj = Assert.IsType<NotFoundObjectResult>(data.Result);
            Assert.Equal($"Categoria com id= {catId} não localizada...", notFoundObj.Value);
        }

        /********************** TESTE POST ***************************/
        [Fact]
        public async Task PostCategoria_AddValidData_Return_CreatedResult()
        {
            // Arrange
            var controller = new CategoriasController(repository, new NullLogger<CategoriasController>(), mapper);
            var cat = new CategoriaDTO()
            {
                Nome = "Teste Unitario Inclusao",
                ImagemUrl = "testecatinclusao.jpg"
            };

            // Act
            var data = await controller.PostCategoria(cat);

            // Assert
            Assert.IsType<CreatedAtRouteResult>(data);
        }

        /********************** TESTE PUT ****************************/
        [Fact]
        public async Task PutCategoria_Update_ValidData_Return_OkResult()
        {
            // Arrange
            var controller = new CategoriasController(repository, new NullLogger<CategoriasController>(), mapper);
            var catId = 4;

            // Act
            var existingPost = await controller.GetCategoriasId(catId);
            var resultPost = existingPost.Result.Should().BeAssignableTo<OkObjectResult>().Subject;
            var result = resultPost.Value.Should().BeAssignableTo<CategoriaDTO>().Subject;

            var catDto = new CategoriaDTO();
            catDto.CategoriaId = catId;
            catDto.Nome = "Categoria Atualizada - Teste Unitario 1";
            catDto.ImagemUrl = result.ImagemUrl;

            var updatedData = await controller.PutCategoria(catId, catDto);

            // Assert
            Assert.IsType<OkResult>(updatedData);
        }

        /********************** TESTE DELETE *************************/
        [Fact]
        public async Task DeleteCategoria_Return_OkResult()
        {
            // Arrange
            var controller = new CategoriasController(repository, new NullLogger<CategoriasController>(), mapper);
            var catId = 4;

            // Act
            var data = await controller.DeleteCategoria(catId);


            // Assert
            var okResult = Assert.IsType<OkObjectResult>(data.Result);
            Assert.IsType<CategoriaDTO>(okResult.Value);
        }
    }
}
