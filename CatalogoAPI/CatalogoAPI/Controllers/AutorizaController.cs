using CatalogoAPI.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CatalogoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorizaController : ControllerBase
    {
        // Instancias necessárias do identity
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        // Instancia do IConfiguration para ler o appsetting.json
        private readonly IConfiguration _configuration;

        public AutorizaController(UserManager<IdentityUser> userManager,
                                SignInManager<IdentityUser> signInManager,
                                IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }


        // Get apenas para verificar se a API está atendendo.
        [HttpGet]
        public ActionResult<string> Get()
        {
            return $"AutorizaController :: Acessado em : {DateTime.Now.ToLongDateString()}";
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] UsuarioDTO model)
        {   
            // Cria uma instancia do usuario do Identity e
            // insere os dados que estão vindo no corpo do request [FromBody]
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                // Está definido como true apenas para neste momento nao ter que
                // enviar uma solicitação para o email do usuário para confirmação
                EmailConfirmed = true
            };


            // Criamos o usuário com as informações recebidas
            // Note que a senha está sendo passada por fora-----------v
            var result = await _userManager.CreateAsync(user, model.Password);

            // ai verificamos a criação
            // se não for feita com sucesso, retornamos um bad request.
            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UsuarioDTO userInfo)
        {
            // Verfica as credenciais do usuário e retorna um valor
            var result = await _signInManager.PasswordSignInAsync(userInfo.Email,
                // o lockoutOnFailure é para bloquear se tentar mais de 3x
                userInfo.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Ok(GeraToken(userInfo));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Login Inválido...");
                return BadRequest(ModelState);
            }
        }

        private UsuarioToken GeraToken(UsuarioDTO userInfo)
        {
            // Define declarações do usuário
            // Não é obrigatorio mas aumenta a segurança
            // Isso seria como informações adicionais que os registros
            // pedem para fazer ao criar a conta, mas está sendo definido
            // diretamente aqui para simplificar
            var claims = new[]
            {
                // Cadastra o email no campo UniqueName
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
                
                // Esses claim na verdade podem ser qualquer coisa.
                // O Claim funciona da seguinte forma:
                // no primeiro campo você define o tipo do campo ou dá um nome,
                // no segundo campo você carrega o dado do campo.
                new Claim("meuPet", "pipoca"),
                
                // Definimos um Guid para o ID do token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Gera uma chave com base em um algoritmo simetrico
            var key = new SymmetricSecurityKey(
                // Usa nossa chave secreta que está no appsettings.json
                Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));

            // Gera a assinatura digital do token usando o
            // algoritmo HMAC e a chave privada
            var credenciais = new SigningCredentials(key,
                SecurityAlgorithms.HmacSha256);

            // Define o tempo de expiração do token.
            // Mais uma vez pega uma definição inserida no appsettings.json
            var expiracao = _configuration["TokenConfiguration:ExpireHours"];
            
            // Aqui convertemos o valor para o formato UTC
            // isso é importante porque se o usuario usar um formato
            // diferente de hora/fuso horario, não vai dar problema.
            var expiration = DateTime.UtcNow.AddHours(double.Parse(expiracao));

            // Classe que representa um token JWT e gera o token
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["TokenConfiguration:Issuer"],
                audience: _configuration["TokenConfiguration:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credenciais);

            // Retorna os dados com o token e informações
            return new UsuarioToken()
            {
                // informa se está autenticado
                Authenticated = true,
                // Serializa o token gerado
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                // data de expiração
                Expiration = expiration,
                Message = "Token JWT OK"
            };
        }
    }
}
