using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Licorera.Models;

namespace Licorera.Controllers
{
    [Authorize(Roles = "Admin,Vendedor")]
    public class PedidoesController : Controller
    {
        private readonly GestionNegocioContext _context;

        public PedidoesController(GestionNegocioContext context)
        {
            _context = context;
        }

        // GET: Pedidoes
        public async Task<IActionResult> Index()
        {
            // Solo mostrar pedidos que NO han sido eliminados por admin
            var pedidos = await _context.Pedidos
                .Where(p => !p.EliminadoPorAdmin)
                .Include(p => p.Cliente)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Producto)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            // Statistics for the view
            ViewBag.TotalPedidos = pedidos.Count;
            ViewBag.PedidosPendientes = pedidos.Count(p => p.Estado == "Pendiente");
            ViewBag.PedidosCompletados = pedidos.Count(p => p.Estado == "Completado");
            ViewBag.PedidosEnProceso = pedidos.Count(p => p.Estado == "En Proceso");
            ViewBag.VentasTotales = pedidos.Where(p => p.Estado == "Completado").Sum(p => p.Total);
            ViewBag.VentasHoy = pedidos
                .Where(p => p.Fecha.HasValue && p.Fecha.Value.Date == DateTime.Today)
                .Sum(p => p.Total);

            return View(pedidos);
        }

        // GET: Pedidoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID de pedido no especificado.";
                return RedirectToAction(nameof(Index));
            }

            var pedido = await _context.Pedidos
                .Where(p => !p.EliminadoPorAdmin)
                .Include(p => p.Cliente)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Producto)
                        .ThenInclude(pr => pr.Categoria)
                .FirstOrDefaultAsync(m => m.PedidoId == id);

            if (pedido == null)
            {
                TempData["ErrorMessage"] = "Pedido no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(pedido);
        }

        // GET: Pedidoes/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes.Select(c => new {
                ClienteId = c.ClienteId,
                DisplayText = $"{c.Nombre} - {c.Email}"
            }), "ClienteId", "DisplayText");
            
            return View();
        }

        // POST: Pedidoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClienteId,Total,Estado")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                pedido.Fecha = DateTime.Now;
                pedido.Estado = pedido.Estado ?? "Pendiente";
                
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Pedido creado exitosamente.";
                return RedirectToAction(nameof(Details), new { id = pedido.PedidoId });
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes.Select(c => new {
                ClienteId = c.ClienteId,
                DisplayText = $"{c.Nombre} - {c.Email}"
            }), "ClienteId", "DisplayText", pedido.ClienteId);
            
            TempData["ErrorMessage"] = "Error al crear el pedido. Por favor, verifica los datos.";
            return View(pedido);
        }

        // GET: Pedidoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID de pedido no especificado.";
                return RedirectToAction(nameof(Index));
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                TempData["ErrorMessage"] = "Pedido no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["ClienteId"] = new SelectList(_context.Clientes.Select(c => new {
                ClienteId = c.ClienteId,
                DisplayText = $"{c.Nombre} - {c.Email}"
            }), "ClienteId", "DisplayText", pedido.ClienteId);
            
            return View(pedido);
        }

        // POST: Pedidoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PedidoId,ClienteId,Fecha,Total,Estado")] Pedido pedido)
        {
            if (id != pedido.PedidoId)
            {
                TempData["ErrorMessage"] = "ID de pedido no válido.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Pedido actualizado exitosamente.";
                    return RedirectToAction(nameof(Details), new { id = pedido.PedidoId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.PedidoId))
                    {
                        TempData["ErrorMessage"] = "El pedido no existe.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error de concurrencia. El pedido fue modificado por otro usuario.";
                        throw;
                    }
                }
            }
            
            ViewData["ClienteId"] = new SelectList(_context.Clientes.Select(c => new {
                ClienteId = c.ClienteId,
                DisplayText = $"{c.Nombre} - {c.Email}"
            }), "ClienteId", "DisplayText", pedido.ClienteId);
            
            TempData["ErrorMessage"] = "Error al actualizar el pedido. Por favor, verifica los datos.";
            return View(pedido);
        }

        // GET: Pedidoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID de pedido no especificado.";
                return RedirectToAction(nameof(Index));
            }

            var pedido = await _context.Pedidos
                .Where(p => !p.EliminadoPorAdmin)
                .Include(p => p.Cliente)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(m => m.PedidoId == id);
                
            if (pedido == null)
            {
                TempData["ErrorMessage"] = "Pedido no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(pedido);
        }

        // POST: Pedidoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.PedidoId == id && !p.EliminadoPorAdmin);
                
            if (pedido != null)
            {
                // En lugar de eliminar físicamente, marcar como eliminado por admin
                pedido.EliminadoPorAdmin = true;
                
                // Opcionalmente, cambiar el estado a "Eliminado" para mayor claridad
                if (pedido.Estado != "Cancelado")
                {
                    pedido.Estado = "Eliminado";
                }
                
                _context.Update(pedido);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Pedido eliminado exitosamente. El cliente aún puede verlo en su historial.";
            }
            else
            {
                TempData["ErrorMessage"] = "Pedido no encontrado.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Update order status
        [HttpPost]
        public async Task<IActionResult> ActualizarEstado(int pedidoId, string nuevoEstado)
        {
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.PedidoId == pedidoId && !p.EliminadoPorAdmin);
                
            if (pedido == null)
            {
                return Json(new { success = false, message = "Pedido no encontrado" });
            }

            try
            {
                pedido.Estado = nuevoEstado;
                await _context.SaveChangesAsync();
                
                return Json(new { 
                    success = true, 
                    message = $"Estado del pedido actualizado a {nuevoEstado}",
                    nuevoEstado = nuevoEstado
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al actualizar el estado: " + ex.Message });
            }
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.PedidoId == id && !e.EliminadoPorAdmin);
        }
    }
}
