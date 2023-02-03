using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoAPI.Controllers
{
    [ApiVersion("2.0")]
    // Deixamos a rota com o mesmo nome para que os 2 controladores
    // possam ser acessados apenas alterando qual a versão,
    // já que não pode existir duas classes com o mesmo nome
    [Route("api/teste")]
    [ApiController]
    public class TesteVersionV2Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("<html><body><h2>TesteVersionV2Controller - 2.0 </h2></body></html>", "text/html");
        }
    }
}
