using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Licorera.Models;
using System.Security.Claims;

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
            // Obtener el ID del cliente actual
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (cliente == null)
            {
                return View(new List<Pedido>());
            }

            var pedidos = await _context.Pedidos
                .Include(p => p.DetallePedidos)
                .ThenInclude(dp => dp.Producto)
                .Where(p => p.ClienteId == cliente.ClienteId)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            return View(pedidos);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            // Obtener el ID del cliente actual
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (cliente == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.DetallePedidos)
                .ThenInclude(dp => dp.Producto)
                .ThenInclude(p => p.Categoria)
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.PedidoId == id && p.ClienteId == cliente.ClienteId);

            if (pedido == null)
            {
                return NotFound();
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