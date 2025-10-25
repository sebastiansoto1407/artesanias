using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace practicamvc.Models
{
    public class PedidoModel
    {
        public int Id { get; set; }

        [Display(Name = "Fecha del pedido")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime FechaPedido { get; set; } = DateTime.Today;

        [Display(Name = "Cliente")]
        [Required(ErrorMessage = "El cliente es obligatorio")]
        public int IdCliente { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente"; 

        [Display(Name = "Monto total (Bs)")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999, ErrorMessage = "El monto debe ser ≥ 0")]
        public decimal MontoTotal { get; set; }

        public ClienteModel? Cliente { get; set; }
        public ICollection<DetallePedidoModel>? Detalles { get; set; } = new List<DetallePedidoModel>();
    }
}
