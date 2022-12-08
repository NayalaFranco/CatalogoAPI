using System.ComponentModel.DataAnnotations;

namespace CatalogoAPI.Validations
{
    // Assim como a Controller, a classe de validação tem que ter Attribute no nome
    // e tem que herdar de ValidationAttribute
    public class PrimeiraLetraMaiusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value,
            ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var primeiraLetra = value.ToString()[0].ToString();
            if (primeiraLetra != primeiraLetra.ToUpper())
            {
                return new ValidationResult("A primeira letra do nome tem que ser maiúscula!");
            }
            return ValidationResult.Success;
        }
    }
}
