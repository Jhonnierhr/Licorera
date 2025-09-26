using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Licorera.Models;
using System.Security.Cryptography;
using System.Text;

namespace Licorera.Controllers
{
    [AllowAnonymous] // Solo para setup inicial - remover en producción
    public class SetupController : Controller
    {
        private readonly GestionNegocioContext _context;
        private readonly ILogger<SetupController> _logger;

        public SetupController(GestionNegocioContext context, ILogger<SetupController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Setup/CreatePersonalAdmin
        public async Task<IActionResult> CreatePersonalAdmin()
        {
            try
            {
                // Verificar si ya existe
                var existingUser = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == "jhonnierhr08@gmail.com");

                if (existingUser != null)
                {
                    return Json(new 
                    { 
                        Success = false, 
                        Message = "El usuario jhonnierhr08@gmail.com ya existe",
                        ExistingUser = new
                        {
                            existingUser.Email,
                            existingUser.Nombre,
                            Role = existingUser.Rol?.Nombre
                        }
                    });
                }

                // Obtener rol de administrador
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Admin");
                if (adminRole == null)
                {
                    return Json(new { Success = false, Message = "Rol de administrador no encontrado" });
                }

                // Crear hash de contraseña
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes("Admin123!"));
                    var passwordHash = Convert.ToBase64String(hashedBytes);

                    var newAdmin = new Usuario
                    {
                        Nombre = "Jhonnier Administrator",
                        Email = "jhonnierhr08@gmail.com",
                        PasswordHash = passwordHash,
                        RolId = adminRole.RolId,
                        CreatedAt = DateTime.Now
                    };

                    _context.Usuarios.Add(newAdmin);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Usuario administrador jhonnierhr08@gmail.com creado exitosamente");

                return Json(new 
                { 
                    Success = true, 
                    Message = "Usuario administrador creado exitosamente",
                    Credentials = new
                    {
                        Email = "jhonnierhr08@gmail.com",
                        Password = "Admin123!"
                    },
                    LoginUrl = "/Account/Login"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario administrador personalizado");
                return Json(new { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        // GET: /Setup/ListAdmins
        public async Task<IActionResult> ListAdmins()
        {
            var admins = await _context.Usuarios
                .Include(u => u.Rol)
                .Where(u => u.Rol.Nombre == "Admin")
                .Select(u => new 
                {
                    u.UsuarioId,
                    u.Email,
                    u.Nombre,
                    u.CreatedAt,
                    Role = u.Rol.Nombre
                })
                .ToListAsync();

            return Json(new
            {
                Success = true,
                Message = $"Se encontraron {admins.Count} administradores",
                Administrators = admins,
                Instructions = "Puedes usar cualquiera de estos emails con la contraseña 'Admin123!' para iniciar sesión"
            });
        }

        // GET: /Setup
        public IActionResult Index()
        {
            return View();
        }
    }
}