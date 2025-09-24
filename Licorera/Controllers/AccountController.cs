using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Licorera.ViewModels;
using Licorera.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Licorera.Controllers
{
    public class AccountController : Controller
    {
        private readonly GestionNegocioContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(GestionNegocioContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var passwordHash = HashPassword(model.Password);
                
                var user = await _context.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == passwordHash);

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Role, user.Rol.Nombre),
                        new Claim("UserId", user.UsuarioId.ToString()),
                        new Claim("UserName", user.Nombre)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        RedirectUri = returnUrl
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Set success message based on user role
                    var welcomeMessage = user.Rol.Nombre switch
                    {
                        "Admin" => $"�Bienvenido Administrador! Has iniciado sesi�n exitosamente.",
                        "Vendedor" => $"�Bienvenido {user.Nombre}! Has iniciado sesi�n como Vendedor.",
                        _ => $"�Bienvenido {user.Nombre}! Has iniciado sesi�n exitosamente."
                    };

                    TempData["SuccessMessage"] = welcomeMessage;

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                // Add error message for failed login
                TempData["ErrorMessage"] = "Credenciales incorrectas. Por favor, verifica tu email y contrase�a.";
                ModelState.AddModelError(string.Empty, "Usuario o contrase�a incorrectos");
            }
            else
            {
                TempData["ErrorMessage"] = "Por favor, completa todos los campos requeridos.";
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            // Solo permitir registro como Cliente
            ViewData["Roles"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Roles.Where(r => r.Nombre == "Cliente"), 
                "RolId", 
                "Nombre");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Validaci�n para asegurar que solo se registren clientes
            var selectedRole = await _context.Roles.FirstOrDefaultAsync(r => r.RolId == model.RolId);
            
            if (selectedRole?.Nombre != "Cliente")
            {
                ModelState.AddModelError("RolId", "Solo se permite registro como Cliente");
                TempData["ErrorMessage"] = "Error en el registro. Solo se permite registro como Cliente.";
                ViewData["Roles"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    _context.Roles.Where(r => r.Nombre == "Cliente"), 
                    "RolId", 
                    "Nombre", 
                    model.RolId);
                return View(model);
            }

            // Validaci�n condicional para campos de cliente (siempre requeridos ahora)
            if (string.IsNullOrWhiteSpace(model.Telefono))
            {
                ModelState.AddModelError("Telefono", "El tel�fono es requerido");
            }
            if (string.IsNullOrWhiteSpace(model.Direccion))
            {
                ModelState.AddModelError("Direccion", "La direcci�n es requerida");
            }

            if (ModelState.IsValid)
            {
                if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Este correo electr�nico ya est� registrado");
                    TempData["ErrorMessage"] = "Este correo electr�nico ya est� registrado. Por favor, usa otro email.";
                    ViewData["Roles"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                        _context.Roles.Where(r => r.Nombre == "Cliente"), 
                        "RolId", 
                        "Nombre", 
                        model.RolId);
                    return View(model);
                }

                var usuario = new Usuario
                {
                    Nombre = $"{model.Nombre} {model.Apellido}",
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    RolId = selectedRole.RolId,
                    CreatedAt = DateTime.Now
                };

                var cliente = new Cliente
                {
                    Nombre = $"{model.Nombre} {model.Apellido}",
                    Email = model.Email,
                    Telefono = model.Telefono,
                    Direccion = model.Direccion,
                    CreatedAt = DateTime.Now
                };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        _context.Usuarios.Add(usuario);
                        await _context.SaveChangesAsync();

                        _context.Clientes.Add(cliente);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        TempData["SuccessMessage"] = $"�Registro exitoso! Bienvenido {usuario.Nombre}. Ya puedes iniciar sesi�n con tus credenciales.";
                        return RedirectToAction(nameof(Login));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        TempData["ErrorMessage"] = "Error al crear la cuenta. Por favor, intente nuevamente.";
                        ModelState.AddModelError(string.Empty, "Error al crear la cuenta. Por favor, intente nuevamente.");
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Por favor, completa todos los campos requeridos correctamente.";
            }

            ViewData["Roles"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Roles.Where(r => r.Nombre == "Cliente"), 
                "RolId", 
                "Nombre", 
                model.RolId);
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Has cerrado sesi�n exitosamente. �Hasta pronto!";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            TempData["ErrorMessage"] = "No tienes permisos para acceder a esta p�gina.";
            return View();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // M�todo temporal para verificar las credenciales del admin - puede remover despu�s
        [AllowAnonymous]
        public async Task<IActionResult> VerifyAdmin()
        {
            var adminUser = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == "admin@grandmasliqueurs.com");

            if (adminUser != null)
            {
                var testPassword = "Admin123!";
                var testHash = HashPassword(testPassword);
                
                return Json(new
                {
                    UserExists = true,
                    Email = adminUser.Email,
                    Role = adminUser.Rol?.Nombre,
                    PasswordMatches = adminUser.PasswordHash == testHash,
                    StoredHash = adminUser.PasswordHash,
                    TestHash = testHash,
                    Message = adminUser.PasswordHash == testHash ? 
                        "Credenciales correctas: admin@grandmasliqueurs.com / Admin123!" : 
                        "Las credenciales no coinciden"
                });
            }

            return Json(new 
            { 
                UserExists = false, 
                Message = "Usuario administrador no encontrado. Reinicie la aplicaci�n para crearlo autom�ticamente." 
            });
        }
    }
}