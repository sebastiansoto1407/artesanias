using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practicamvc.Data;
using practicamvc.Models;
using practicamvc.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace practicamvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly ArtesaniasContext _context;

        public AccountController(ArtesaniasContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Buscar usuario por email
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Email no registrado.");
                    return View(model);
                }

                // Verificar si el usuario está activo
                if (!usuario.EstaActivo)
                {
                    ModelState.AddModelError(string.Empty, "Tu cuenta está desactivada. Contacta al administrador.");
                    return View(model);
                }

                // Verificar contraseña (en producción usar BCrypt o similar)
                if (usuario.Password != model.Password)
                {
                    ModelState.AddModelError(string.Empty, "Contraseña incorrecta.");
                    return View(model);
                }

                // Crear claims del usuario
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Rol)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(2)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirigir según el returnUrl o a Home
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!model.EsMayorDeEdad())
            {
                ModelState.AddModelError("FechaNacimiento", "Debes ser mayor de 18 años.");
            }

            if (model.FechaNacimiento > DateTime.Today)
            {
                ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento no puede ser futura.");
            }

            if (ModelState.IsValid)
            {
                var usuario = new UsuarioModel
                {
                    NombreCompleto = model.NombreCompleto,
                    Email = model.Email,
                    Password = model.Password, // En producción: hashear con BCrypt
                    FechaNacimiento = model.FechaNacimiento,
                    Rol = "Cliente", // Rol por defecto
                    FechaRegistro = DateTime.Now,
                    EstaActivo = true
                };

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registro exitoso. Ya puedes iniciar sesión.";
                return RedirectToAction(nameof(Login));
            }

            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Validación remota para email único en registro
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> CheckEmailUnicoRegistro(string email)
        {
            var existeEmail = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == email.ToLower());

            if (existeEmail)
            {
                return Json($"El email '{email}' ya está registrado.");
            }

            return Json(true);
        }
    }
}
