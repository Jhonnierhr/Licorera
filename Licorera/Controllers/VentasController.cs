using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Licorera.Models;

namespace Licorera.Controllers
{
    [Authorize(Policy = "RequireAdminOrVendedorRole")]
    public class VentasController : Controller
    {
        private readonly GestionNegocioContext _context;

        public VentasController(GestionNegocioContext context)
        {
            _context = context;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .Include(v => v.DetalleVenta)
                .ThenInclude(dv => dv.Producto)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();

            return View(ventas);
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .Include(v => v.DetalleVenta)
                .ThenInclude(dv => dv.Producto)
                .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.VentaId == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Nombre");
            ViewData["Productos"] = _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Stock > 0)
                .ToList();
            return View();
        }

        // POST: Ventas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VentaId,ClienteId,UsuarioId,Fecha,Total,Estado")] Venta venta)
        {
            if (ModelState.IsValid)
            {
                venta.Fecha = DateTime.Now;
                _context.Add(venta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre", venta.ClienteId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Nombre", venta.UsuarioId);
            return View(venta);
        }

        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre", venta.ClienteId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Nombre", venta.UsuarioId);
            return View(venta);
        }

        // POST: Ventas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VentaId,ClienteId,UsuarioId,Fecha,Total,Estado")] Venta venta)
        {
            if (id != venta.VentaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(venta.VentaId))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre", venta.ClienteId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Nombre", venta.UsuarioId);
            return View(venta);
        }

        // POST: Cambiar estado de venta
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int ventaId, string nuevoEstado)
        {
            var venta = await _context.Ventas.FindAsync(ventaId);
            if (venta == null)
            {
                return Json(new { success = false, message = "Venta no encontrada" });
            }

            venta.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Estado cambiado a {nuevoEstado}" });
        }

        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .Include(v => v.DetalleVenta)
                .ThenInclude(dv => dv.Producto)
                .FirstOrDefaultAsync(m => m.VentaId == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.DetalleVenta)
                .FirstOrDefaultAsync(v => v.VentaId == id);

            if (venta != null)
            {
                // Eliminar primero los detalles
                _context.DetalleVentas.RemoveRange(venta.DetalleVenta);
                // Luego eliminar la venta
                _context.Ventas.Remove(venta);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.VentaId == id);
        }
    }
}
