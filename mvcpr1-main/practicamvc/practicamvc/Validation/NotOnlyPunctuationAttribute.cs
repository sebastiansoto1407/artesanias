using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace practicamvc.Validation
{
    /// <summary>
    /// Falla si el texto está vacío/espacios o si está compuesto solo por signos/puntuación.
    /// Permite letras (incluye acentos), números y espacios normales.
    /// </summary>
    public class NotOnlyPunctuationAttribute : ValidationAttribute
    {
        public int MinLength { get; set; } = 2;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var s = (value as string)?.Trim() ?? string.Empty;
            if (s.Length < MinLength)
                return new ValidationResult($"Debe tener al menos {MinLength} caracteres.");

            var onlyPunct = Regex.Replace(s, @"[\p{L}\p{N}\s]+", string.Empty); 
            var keep = Regex.Replace(s, @"[^\p{L}\p{N}\s]+", string.Empty);

            if (keep.Length == 0)
                return new ValidationResult("No se permiten valores compuestos solo por signos o puntuación.");

            return ValidationResult.Success;
        }
    }
}
