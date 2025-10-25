using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using practicamvc.Data;
using practicamvc.Models;

namespace practicamvc.Controllers
{
    public class PedidosController : Controller
    {
        private readonly ArtesaniasContext _context;

        public PedidosController(ArtesaniasContext context) => _context = context;

        private static readonly string[] Estados = new[] { "Pendiente", "Pagado", "Enviado", "Cancelado" };

        public async Task<IActionResult> Index(string? qEstado, int? idCliente)
        {
            var query = _context.Pedidos
                .Include(p => p.Cliente)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(qEstado))
                query = query.Where(p => p.Estado == qEstado);

            if (idCliente.HasValue)
                query = query.Where(p => p.IdCliente == idCliente.Value);

            var data = await query
                .OrderByDescending(p => p.FechaPedido)
                .ThenBy(p => p.Id)
                .ToListAsync();

            ViewBag.Estados = new SelectList(Estados);
            ViewBag.Clientes = new SelectList(await _context.Clientes.OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre");
            ViewBag.qEstado = qEstado;
            ViewBag.idCliente = idCliente;

            return View(data);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido is null) return NotFound();

            return View(pedido);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Clientes = new SelectList(await _context.Clientes.OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre");
            ViewBag.Estados = new SelectList(Estados);
            return View(new PedidoModel { FechaPedido = DateTime.Today, Estado = "Pendiente" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PedidoModel pedido)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = new SelectList(await _context.Clientes.OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre", pedido.IdCliente);
                ViewBag.Estados = new SelectList(Estados, pedido.Estado);
                return View(pedido);
            }

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Pedido creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido is null) return NotFound();

            ViewBag.Clientes = new SelectList(await _context.Clientes.OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre", pedido.IdCliente);
            ViewBag.Estados = new SelectList(Estados, pedido.Estado);
            return View(pedido);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PedidoModel pedido)
        {
            if (id != pedido.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = new SelectList(await _context.Clientes.OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre", pedido.IdCliente);
                ViewBag.Estados = new SelectList(Estados, pedido.Estado);
                return View(pedido);
            }

            try
            {
                _context.Update(pedido);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Pedido actualizado.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Pedidos.AnyAsync(e => e.Id == pedido.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido is null) return NotFound();
            return View(pedido);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Pedido eliminado.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
