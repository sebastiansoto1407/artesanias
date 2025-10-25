using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using practicamvc.Validation;

namespace practicamvc.Models
{
    public class ProductoModel
    {
        public int Id { get; set; }
        public ICollection<DetallePedidoModel>? Detalles { get; set; }

        [Display(Name = "Nombre del producto")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(120, MinimumLength = 2, ErrorMessage = "Entre 2 y 120 caracteres.")]
        [NotOnlyPunctuation(MinLength = 2, ErrorMessage = "Ingrese un nombre valido (no solo signos).")]
        [Remote(action: "CheckNombreUnico", controller: "Productos", AdditionalFields = nameof(Id),
            ErrorMessage = "Ya existe un producto con ese nombre.")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Descripcion")]
        [StringLength(500, ErrorMessage = "Maximo 500 caracteres.")]
        [NotOnlyPunctuation(MinLength = 0, ErrorMessage = "Descripcion invalida (no solo signos).")]
        public string? Descripcion { get; set; }

        [Display(Name = "Precio (Bs)")]
        [Range(0.01, 999999, ErrorMessage = "El precio debe ser mayor a 0.")]
        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        public decimal Precio { get; set; }

        [Display(Name = "Stock")]
        [Range(0, 1000000, ErrorMessage = "El stock debe ser ≥ 0.")]
        public int Stock { get; set; }
    }
}
