using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using practicamvc.Data;
using practicamvc.Models;

namespace practicamvc.Controllers
{
    public class DetallesPedidoController : Controller
    {
        private readonly ArtesaniasContext _context;

        public DetallesPedidoController(ArtesaniasContext context) => _context = context;

        public async Task<IActionResult> Index(int? idPedido)
        {
            var query = _context.DetallesPedido
                .Include(d => d.Pedido).ThenInclude(p => p.Cliente)
                .Include(d => d.Producto)
                .AsQueryable();

            if (idPedido.HasValue) query = query.Where(d => d.IdPedido == idPedido.Value);

            var data = await query
                .OrderByDescending(d => d.IdPedido)
                .ThenBy(d => d.Id)
                .ToListAsync();

            var pedidos = await _context.Pedidos.Include(p => p.Cliente).OrderByDescending(p => p.Id).ToListAsync();
            var pedidosList = pedidos.Select(p => new
            {
                p.Id,
                Texto = $"Pedido #{p.Id} — {(p.Cliente != null ? p.Cliente.Nombre : "Sin cliente")}"
            }).ToList();

            ViewBag.Pedidos = new SelectList(pedidosList, "Id", "Texto", idPedido);

            return View(data);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            var detalle = await _context.DetallesPedido
                .Include(d => d.Pedido).ThenInclude(p => p.Cliente)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (detalle is null) return NotFound();
            return View(detalle);
        }

        public async Task<IActionResult> Create(int? idPedido)
        {
            await CargarCombos(idPedido);
            return View(new DetallePedidoModel { Cantidad = 1 });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DetallePedidoModel detalle)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombos(detalle.IdPedido);
                return View(detalle);
            }

            _context.DetallesPedido.Add(detalle);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Detalle agregado.";
            return RedirectToAction(nameof(Index), new { idPedido = detalle.IdPedido });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var detalle = await _context.DetallesPedido.FindAsync(id);
            if (detalle is null) return NotFound();

            await CargarCombos(detalle.IdPedido);
            return View(detalle);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DetallePedidoModel detalle)
        {
            if (id != detalle.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                await CargarCombos(detalle.IdPedido);
                return View(detalle);
            }

            try
            {
                _context.Update(detalle);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Detalle actualizado.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.DetallesPedido.AnyAsync(e => e.Id == detalle.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index), new { idPedido = detalle.IdPedido });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var detalle = await _context.DetallesPedido
                .Include(d => d.Pedido).ThenInclude(p => p.Cliente)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (detalle is null) return NotFound();
            return View(detalle);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detalle = await _context.DetallesPedido.FindAsync(id);
            if (detalle != null)
            {
                var idPedido = detalle.IdPedido;
                _context.DetallesPedido.Remove(detalle);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Detalle eliminado.";
                return RedirectToAction(nameof(Index), new { idPedido });
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCombos(int? idPedidoSeleccionado)
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            var pedidosList = pedidos.Select(p => new
            {
                p.Id,
                Texto = $"Pedido #{p.Id} — {(p.Cliente != null ? p.Cliente.Nombre : "Sin cliente")}"
            });

            ViewBag.Pedidos = new SelectList(pedidosList, "Id", "Texto", idPedidoSeleccionado);

            var productos = await _context.Productos
                .OrderBy(pr => pr.Nombre)
                .ToListAsync();

            ViewBag.Productos = new SelectList(productos, "Id", "Nombre");
        }
    }
}
