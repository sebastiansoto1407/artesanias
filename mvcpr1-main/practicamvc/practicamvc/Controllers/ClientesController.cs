using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practicamvc.Data;
using practicamvc.Models;
using Microsoft.AspNetCore.Authorization;

namespace practicamvc.Controllers
{
    [Authorize(Roles = "Administrador,Vendedor")] // Solo ADMIN y VENDEDOR
    public class ClientesController : Controller
    {
        private readonly ArtesaniasContext _context;

        public ClientesController(ArtesaniasContext context) => _context = context;

        // GET: Clientes
        public async Task<IActionResult> Index(string? q)
        {
            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var n = q.Trim();
                query = query.Where(c =>
                    c.Nombre.Contains(n) ||
                    c.Email.Contains(n) ||
                    (c.Direccion ?? "").Contains(n) ||
                    c.Telefono.Contains(n));
            }

            var data = await query.OrderBy(c => c.Nombre).ToListAsync();
            ViewBag.Q = q;
            return View(data);
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            if (cliente is null) return NotFound();

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create() => View(new ClienteModel());

        // POST: Clientes/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteModel cliente)
        {
            if (!ModelState.IsValid) return View(cliente);

            var emailExiste = await _context.Clientes
                .AnyAsync(c => c.Email.ToUpper() == cliente.Email.ToUpper());
            if (emailExiste)
            {
                ModelState.AddModelError(nameof(cliente.Email), "Ya existe un cliente con ese email.");
                return View(cliente);
            }

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Cliente '{cliente.Nombre}' creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente is null) return NotFound();
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteModel cliente)
        {
            if (id != cliente.Id) return NotFound();
            if (!ModelState.IsValid) return View(cliente);

            var emailDuplicado = await _context.Clientes
                .AnyAsync(c => c.Id != cliente.Id && c.Email.ToUpper() == cliente.Email.ToUpper());
            if (emailDuplicado)
            {
                ModelState.AddModelError(nameof(cliente.Email), "Ya existe otro cliente con ese email.");
                return View(cliente);
            }

            try
            {
                _context.Update(cliente);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cliente '{cliente.Nombre}' actualizado correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Clientes.AnyAsync(e => e.Id == cliente.Id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Clientes/Delete/5 - Solo ADMINISTRADOR
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.Id == id);
            if (cliente is null) return NotFound();
            return View(cliente);
        }

        // POST: Clientes/Delete/5 - Solo ADMINISTRADOR
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cliente '{cliente.Nombre}' eliminado correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
