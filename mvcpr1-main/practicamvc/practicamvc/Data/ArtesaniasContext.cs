using Microsoft.EntityFrameworkCore;
using practicamvc.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace practicamvc.Data
{
    public class ArtesaniasContext : DbContext
    {
        public ArtesaniasContext(DbContextOptions<ArtesaniasContext> options)
            : base(options) { }

        public DbSet<ProductoModel> Productos { get; set; }
        public DbSet<ClienteModel> Clientes { get; set; }
        public DbSet<PedidoModel> Pedidos { get; set; }
        public DbSet<DetallePedidoModel> DetallesPedido { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PedidoModel>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos) 
                .HasForeignKey(p => p.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetallePedidoModel>()
                .HasOne(d => d.Pedido)
                .WithMany(p => p.Detalles)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetallePedidoModel>()
                .HasOne(d => d.Producto)
                .WithMany(p => p.Detalles) 
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PedidoModel>().Property(p => p.MontoTotal).HasPrecision(18, 2);
            modelBuilder.Entity<DetallePedidoModel>().Property(d => d.PrecioUnitario).HasPrecision(18, 2);
            modelBuilder.Entity<ProductoModel>().Property(p => p.Precio).HasPrecision(18, 2);
        }

        public override int SaveChanges()
        {
            RecalcularMontos();
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            RecalcularMontos();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void RecalcularMontos()
        {
            var ids = ChangeTracker.Entries<DetallePedidoModel>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .Select(e => e.Entity.IdPedido)
                .Distinct()
                .ToList();

            foreach (var idPedido in ids)
            {
                var pedido = Pedidos.Include(p => p.Detalles).FirstOrDefault(p => p.Id == idPedido);
                if (pedido != null)
                    pedido.MontoTotal = pedido.Detalles?.Sum(d => d.Cantidad * d.PrecioUnitario) ?? 0m;
            }
        }
    }
}
