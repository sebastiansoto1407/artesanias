using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using practicamvc.Data;
using practicamvc.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace practicamvc.Controllers
{
    [Authorize(Roles = "Administrador,Vendedor")] // Solo ADMIN y VENDEDOR
    public class DetallesPedidoController : Controller
    {
        private readonly ArtesaniasContext _context;

        public DetallesPedidoController(ArtesaniasContext context)
        {
            _context = context;
        }

        // GET: DetallesPedido
        public async Task<IActionResult> Index()
        {
            var detalles = await _context.DetallesPedido
                .Include(d => d.Pedido)
                    .ThenInclude(p => p.Cliente)
                .Include(d => d.Producto)
                .ToListAsync();
            return View(detalles);
        }

        // GET: DetallesPedido/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var detalle = await _context.DetallesPedido
                .Include(d => d.Pedido)
                    .ThenInclude(p => p.Cliente)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (detalle == null) return NotFound();

            return View(detalle);
        }

        // GET: DetallesPedido/Create
        public IActionResult Create()
        {
            ViewData["IdPedido"] = new SelectList(_context.Pedidos, "Id", "Id");
            ViewData["IdProducto"] = new SelectList(_context.Productos, "Id", "Nombre");
            return View();
        }

        // POST: DetallesPedido/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdPedido,IdProducto,Cantidad,PrecioUnitario")] DetallePedidoModel detalle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detalle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Detalle de pedido creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPedido"] = new SelectList(_context.Pedidos, "Id", "Id", detalle.IdPedido);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "Id", "Nombre", detalle.IdProducto);
            return View(detalle);
        }

        // GET: DetallesPedido/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var detalle = await _context.DetallesPedido.FindAsync(id);
            if (detalle == null) return NotFound();

            ViewData["IdPedido"] = new SelectList(_context.Pedidos, "Id", "Id", detalle.IdPedido);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "Id", "Nombre", detalle.IdProducto);
            return View(detalle);
        }

        // POST: DetallesPedido/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdPedido,IdProducto,Cantidad,PrecioUnitario")] DetallePedidoModel detalle)
        {
            if (id != detalle.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detalle);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Detalle de pedido actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetalleExists(detalle.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPedido"] = new SelectList(_context.Pedidos, "Id", "Id", detalle.IdPedido);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "Id", "Nombre", detalle.IdProducto);
            return View(detalle);
        }

        // GET: DetallesPedido/Delete/5 - Solo ADMINISTRADOR
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var detalle = await _context.DetallesPedido
                .Include(d => d.Pedido)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detalle == null) return NotFound();

            return View(detalle);
        }

        // POST: DetallesPedido/Delete/5 - Solo ADMINISTRADOR
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detalle = await _context.DetallesPedido.FindAsync(id);
            if (detalle != null)
            {
                _context.DetallesPedido.Remove(detalle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Detalle de pedido eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DetalleExists(int id)
        {
            return _context.DetallesPedido.Any(e => e.Id == id);
        }
    }
}
