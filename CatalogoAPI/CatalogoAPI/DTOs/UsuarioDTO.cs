using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace CatalogoAPI.DTOs
{
    public class UsuarioDTO
    {
        // Essas DataAnnotations não são necessárias,
        // o Identity ja faz a filtragem os dados inseridos,
        // eu coloquei apenas para deixar em português os erros.
        [Required(ErrorMessage = "Email é requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password é obrigatória")]
        [StringLength(30, ErrorMessage = "A {0} deve ter no mínimo {2} e no máximo " +
           "{1} caracteres.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Você tem que confirmar a senha")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Senha e confirmação não conferem.")]
        public string ConfirmPassword { get; set; }
    }
}
