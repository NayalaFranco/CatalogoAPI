namespace CatalogoAPI.DTOs
{
    public class UsuarioToken
    {
        // Se está autenticado ou não
        public bool Authenticated { get; set; }

        // Quando expira
        public DateTime Expiration { get; set; }

        // O Token em si
        public string Token { get; set; }

        // Mensagem que pode ser de erro, gerado com sucesso, etc
        public string Message { get; set; }
    }
}
