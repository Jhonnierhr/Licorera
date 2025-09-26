using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Licorera.Models;

namespace Licorera.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class MisPedidosController : Controller
    {
        private readonly GestionNegocioContext _context;

        public MisPedidosController(GestionNegocioContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (cliente == null)
            {
                TempData["ErrorMessage"] = "Cliente no encontrado.";
                return RedirectToAction("Index", "Home");
            }

            // Mostrar TODOS los pedidos del cliente, incluyendo los eliminados por admin
            // El cliente debe poder ver su historial completo
            var pedidos = await _context.Pedidos
                .Where(p => p.ClienteId == cliente.ClienteId)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Producto)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            return View(pedidos);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (cliente == null)
            {
                TempData["ErrorMessage"] = "Cliente no encontrado.";
                return RedirectToAction("Index", "Home");
            }

            // Permitir ver detalles de cualquier pedido del cliente, incluso si fue eliminado por admin
            var pedido = await _context.Pedidos
                .Where(p => p.ClienteId == cliente.ClienteId && p.PedidoId == id)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Producto)
                        .ThenInclude(pr => pr.Categoria)
                .FirstOrDefaultAsync();

            if (pedido == null)
            {
                TempData["ErrorMessage"] = "Pedido no encontrado.";
                return RedirectToAction("Index");
            }

            return View(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> CancelarPedido(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (cliente == null)
            {
                return Json(new { success = false, message = "Cliente no encontrado" });
            }

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.PedidoId == id && p.ClienteId == cliente.ClienteId);

            if (pedido == null)
            {
                return Json(new { success = false, message = "Pedido no encontrado" });
            }

            // No permitir cancelar pedidos que ya fueron eliminados por admin
            if (pedido.EliminadoPorAdmin)
            {
                return Json(new { success = false, message = "Este pedido ya no puede ser modificado" });
            }

            if (pedido.Estado == "Completado" || pedido.Estado == "Cancelado")
            {
                return Json(new { success = false, message = "No se puede cancelar este pedido" });
            }

            pedido.Estado = "Cancelado";
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Pedido cancelado exitosamente" });
        }
    }
}