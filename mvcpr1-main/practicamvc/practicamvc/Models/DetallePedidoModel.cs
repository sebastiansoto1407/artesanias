using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace practicamvc.Models
{
    public class DetallePedidoModel
    {
        public int Id { get; set; }

        [Display(Name = "Pedido")]
        [Required]
        public int IdPedido { get; set; }

        [Display(Name = "Producto")]
        [Required]
        public int IdProducto { get; set; }

        [Display(Name = "Cantidad")]
        [Range(1, 100000, ErrorMessage = "Cantidad debe ser ≥ 1")]
        public int Cantidad { get; set; }

        [Display(Name = "Precio unitario (Bs)")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999, ErrorMessage = "El precio debe ser ≥ 0")]
        public decimal PrecioUnitario { get; set; }

        public PedidoModel? Pedido { get; set; }
        public ProductoModel? Producto { get; set; }
    }
}
