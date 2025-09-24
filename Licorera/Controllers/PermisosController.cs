using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Licorera.Models;
using System.Security.Cryptography;
using System.Text;

namespace Licorera.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PermisosController : Controller
    {
        private readonly GestionNegocioContext _context;

        public PermisosController(GestionNegocioContext context)
        {
            _context = context;
        }

        // GET: Permisos
        public async Task<IActionResult> Index(string tab = "usuarios")
        {
            ViewBag.ActiveTab = tab;
            
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var roles = await _context.Roles.ToListAsync();

            ViewBag.Roles = roles;
            ViewBag.TotalUsuarios = usuarios.Count;
            ViewBag.TotalVendedores = usuarios.Count(u => u.Rol.Nombre == "Vendedor");
            ViewBag.TotalClientes = usuarios.Count(u => u.Rol.Nombre == "Cliente");
            ViewBag.TotalAdmins = usuarios.Count(u => u.Rol.Nombre == "Admin");

            return View(usuarios);
        }

        // GET: Permisos/Details/5 - Ver detalles de usuario
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
                
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Permisos/Create - Crear nuevo usuario
        public IActionResult Create()
        {
            ViewBag.Roles = _context.Roles.Where(r => r.Nombre != "Admin").ToList();
            return View();
        }

        // POST: Permisos/Create - Crear nuevo usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Email,PasswordHash,RolId")] Usuario usuario, string Password)
        {
            if (ModelState.IsValid)
            {
                // Verificar que el email no exista
                var existeUsuario = await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email);
                if (existeUsuario)
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario con este email");
                    ViewBag.Roles = _context.Roles.Where(r => r.Nombre != "Admin").ToList();
                    return View(usuario);
                }

                // Hashear la contraseña
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));
                    usuario.PasswordHash = Convert.ToBase64String(hashedBytes);
                }

                usuario.CreatedAt = DateTime.Now;
                
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Usuario creado exitosamente";
                return RedirectToAction(nameof(Index), new { tab = "usuarios" });
            }
            
            ViewBag.Roles = _context.Roles.Where(r => r.Nombre != "Admin").ToList();
            return View(usuario);
        }

        // GET: Permisos/Edit/5 - Editar usuario
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
            
            ViewBag.Roles = _context.Roles.Where(r => r.Nombre != "Admin").ToList();
            return View(usuario);
        }

        // POST: Permisos/Edit/5 - Editar usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,Nombre,Email,RolId,CreatedAt")] Usuario usuario, string Password)
        {
            if (id != usuario.UsuarioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioExistente = await _context.Usuarios.FindAsync(id);
                    if (usuarioExistente == null)
                    {
                        return NotFound();
                    }

                    usuarioExistente.Nombre = usuario.Nombre;
                    usuarioExistente.Email = usuario.Email;
                    usuarioExistente.RolId = usuario.RolId;

                    // Solo actualizar contraseña si se proporciona una nueva
                    if (!string.IsNullOrEmpty(Password))
                    {
                        using (var sha256 = SHA256.Create())
                        {
                            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));
                            usuarioExistente.PasswordHash = Convert.ToBase64String(hashedBytes);
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Usuario actualizado exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { tab = "usuarios" });
            }
            
            ViewBag.Roles = _context.Roles.Where(r => r.Nombre != "Admin").ToList();
            return View(usuario);
        }

        // GET: Permisos/Delete/5 - Eliminar usuario
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
                
            if (usuario == null)
            {
                return NotFound();
            }

            // No permitir eliminar administradores
            if (usuario.Rol.Nombre == "Admin")
            {
                TempData["ErrorMessage"] = "No se puede eliminar un usuario administrador";
                return RedirectToAction(nameof(Index), new { tab = "usuarios" });
            }

            return View(usuario);
        }

        // POST: Permisos/Delete/5 - Confirmar eliminación de usuario
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.UsuarioId == id);
            if (usuario != null)
            {
                // No permitir eliminar administradores
                if (usuario.Rol.Nombre == "Admin")
                {
                    TempData["ErrorMessage"] = "No se puede eliminar un usuario administrador";
                    return RedirectToAction(nameof(Index), new { tab = "usuarios" });
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Usuario eliminado exitosamente";
            }

            return RedirectToAction(nameof(Index), new { tab = "usuarios" });
        }

        [HttpPost]
        public async Task<IActionResult> CambiarRol(int usuarioId, int nuevoRolId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            // No permitir cambiar roles de administradores
            if (usuario.Rol.Nombre == "Admin")
            {
                return Json(new { success = false, message = "No se puede cambiar el rol de un administrador" });
            }

            var nuevoRol = await _context.Roles.FirstOrDefaultAsync(r => r.RolId == nuevoRolId);
            if (nuevoRol == null)
            {
                return Json(new { success = false, message = "Rol no encontrado" });
            }

            // No permitir asignar rol de Admin
            if (nuevoRol.Nombre == "Admin")
            {
                return Json(new { success = false, message = "No se puede asignar el rol de administrador" });
            }

            try
            {
                var rolAnterior = usuario.Rol.Nombre;
                usuario.RolId = nuevoRolId;
                await _context.SaveChangesAsync();

                return Json(new 
                { 
                    success = true, 
                    message = $"Rol cambiado exitosamente de {rolAnterior} a {nuevoRol.Nombre}",
                    nuevoRol = nuevoRol.Nombre
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al cambiar el rol: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AsignarVendedor(int usuarioId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            var rolVendedor = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Vendedor");
            if (rolVendedor == null)
            {
                return Json(new { success = false, message = "Rol de Vendedor no encontrado" });
            }

            try
            {
                var rolAnterior = usuario.Rol.Nombre;
                usuario.RolId = rolVendedor.RolId;
                await _context.SaveChangesAsync();

                return Json(new 
                { 
                    success = true, 
                    message = $"{usuario.Nombre} ahora es Vendedor",
                    rolAnterior = rolAnterior,
                    nuevoRol = "Vendedor"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al asignar el rol: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RevocarVendedor(int usuarioId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            var rolCliente = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Cliente");
            if (rolCliente == null)
            {
                return Json(new { success = false, message = "Rol de Cliente no encontrado" });
            }

            try
            {
                usuario.RolId = rolCliente.RolId;
                await _context.SaveChangesAsync();

                return Json(new 
                { 
                    success = true, 
                    message = $"{usuario.Nombre} ya no es Vendedor",
                    rolAnterior = "Vendedor",
                    nuevoRol = "Cliente"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al revocar el rol: " + ex.Message });
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}