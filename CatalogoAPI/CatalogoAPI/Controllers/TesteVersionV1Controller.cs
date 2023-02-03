using Microsoft.AspNetCore.Mvc;

namespace CatalogoAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/teste")]
    [ApiController]
    public class TesteVersionV1Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("<html><body><h2>TesteVersionV1Controller - 1.0 </h2></body></html>", "text/html");
        }
    }
}
