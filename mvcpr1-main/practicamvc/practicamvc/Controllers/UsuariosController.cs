using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practicamvc.Data;
using practicamvc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace practicamvc.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ArtesaniasContext _context;

        public UsuariosController(ArtesaniasContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .OrderByDescending(u => u.FechaRegistro)
                .ToListAsync();
            return View(usuarios);
        }

        // GET: 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            var usuario = new UsuarioModel
            {
                FechaRegistro = DateTime.Now,
                EstaActivo = true,
                Rol = "Cliente"
            };
            return View(usuario);
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NombreCompleto,Email,Password,Rol,FechaNacimiento,FechaRegistro,EstaActivo")] UsuarioModel usuario)
        {
            if (!usuario.EsMayorDeEdad())
            {
                ModelState.AddModelError("FechaNacimiento", "Debes ser mayor de 18 años para registrarte.");
            }

            if (usuario.FechaNacimiento > DateTime.Today)
            {
                ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento no puede ser una fecha futura.");
            }

            if (ModelState.IsValid)
            {
                usuario.FechaRegistro = DateTime.Now;

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Usuario '{usuario.NombreCompleto}' creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreCompleto,Email,Password,Rol,FechaNacimiento,FechaRegistro,EstaActivo")] UsuarioModel usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (!usuario.EsMayorDeEdad())
            {
                ModelState.AddModelError("FechaNacimiento", "Debes ser mayor de 18 años.");
            }

            if (usuario.FechaNacimiento > DateTime.Today)
            {
                ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento no puede ser una fecha futura.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Usuario '{usuario.NombreCompleto}' actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Usuario '{usuario.NombreCompleto}' eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> CheckEmailUnico(string email, int id)
        {
            var existeEmail = await _context.Usuarios
                .AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.Id != id);

            if (existeEmail)
            {
                return Json($"El email '{email}' ya esta registrado en el sistema.");
            }

            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            usuario.EstaActivo = !usuario.EstaActivo;
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            var estado = usuario.EstaActivo ? "activado" : "desactivado";
            TempData["SuccessMessage"] = $"Usuario '{usuario.NombreCompleto}' {estado} exitosamente.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CambiarPassword(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(int id, string Password)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            {
                ModelState.AddModelError("Password", "La contraseña debe tener al menos 6 caracteres.");
                return View(usuario);
            }

            usuario.Password = Password;
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Contraseña actualizada exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
