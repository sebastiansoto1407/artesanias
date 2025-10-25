using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace practicamvc.Models
{
    public class ClienteModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre completo")]
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(160)]
        public string Email { get; set; } = "";

        [Display(Name = "Telefono")]
        [Required(ErrorMessage = "El telefono es obligatorio")]
        [StringLength(25)]
        public string Telefono { get; set; } = "";

        [Display(Name = "Direccion")]
        [StringLength(200, ErrorMessage = "Maximo 200 caracteres")]
        public string? Direccion { get; set; }

        public ICollection<PedidoModel>? Pedidos { get; set; }
    }
}
