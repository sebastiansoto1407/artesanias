using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using practicamvc.Validation;

namespace practicamvc.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre completo")]
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
        [NotOnlyPunctuation(MinLength = 2, ErrorMessage = "Ingrese un nombre válido (no solo signos).")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [StringLength(160, ErrorMessage = "El email no puede exceder 160 caracteres.")]
        [Remote(action: "CheckEmailUnico", controller: "Usuarios", AdditionalFields = nameof(Id),
            ErrorMessage = "Ya existe un usuario con ese email.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial.")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "El rol es obligatorio.")]
        [StringLength(20)]
        [RegularExpression("^(Administrador|Vendedor|Cliente)$",
            ErrorMessage = "El rol debe ser Administrador, Vendedor o Cliente.")]
        public string Rol { get; set; } = "Cliente";

        [Display(Name = "Fecha de nacimiento")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaNacimiento { get; set; }

        [Display(Name = "Fecha de registro")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Display(Name = "¿Est activo?")]
        public bool EstaActivo { get; set; } = true;

        public bool EsMayorDeEdad()
        {
            var edad = DateTime.Today.Year - FechaNacimiento.Year;
            if (FechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;
            return edad >= 18;
        }
    }
}
