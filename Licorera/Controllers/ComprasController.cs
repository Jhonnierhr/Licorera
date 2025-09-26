using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Licorera.Models;
using System.Security.Claims;

namespace Licorera.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ComprasController : Controller
    {
        private readonly GestionNegocioContext _context;

        public ComprasController(GestionNegocioContext context)
        {
            _context = context;
        }

        // GET: Compras
        public async Task<IActionResult> Index()
        {
            var compras = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.Producto)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            return View(compras);
        }

        // GET: Compras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.Producto)
                .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.CompraId == id);

            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // GET: Compras/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Proveedores"] = new SelectList(
                await _context.Proveedores.ToListAsync(), 
                "ProveedorId", 
                "Nombre");
            
            ViewData["Productos"] = await _context.Productos
                .Include(p => p.Categoria)
                .ToListAsync();

            return View();
        }

        // POST: Compras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int proveedorId, List<int> productos, List<int> cantidades, List<decimal> precios)
        {
            if (proveedorId <= 0 || productos == null || productos.Count == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un proveedor y al menos un producto");
                ViewData["Proveedores"] = new SelectList(await _context.Proveedores.ToListAsync(), "ProveedorId", "Nombre");
                ViewData["Productos"] = await _context.Productos.Include(p => p.Categoria).ToListAsync();
                return View();
            }

            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var total = 0m;

                // Calcular total
                for (int i = 0; i < productos.Count; i++)
                {
                    if (cantidades[i] > 0 && precios[i] > 0)
                    {
                        total += cantidades[i] * precios[i];
                    }
                }

                var compra = new Compra
                {
                    ProveedorId = proveedorId,
                    UsuarioId = userId,
                    Fecha = DateTime.Now,
                    Total = total,
                    Estado = "Pendiente"
                };

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                // Agregar detalles de compra
                for (int i = 0; i < productos.Count; i++)
                {
                    if (cantidades[i] > 0 && precios[i] > 0)
                    {
                        var detalle = new DetalleCompra
                        {
                            CompraId = compra.CompraId,
                            ProductoId = productos[i],
                            Cantidad = cantidades[i],
                            PrecioUnitario = precios[i]
                        };

                        _context.DetalleCompras.Add(detalle);
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Compra creada exitosamente";
                return RedirectToAction(nameof(Details), new { id = compra.CompraId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear la compra: " + ex.Message);
                ViewData["Proveedores"] = new SelectList(await _context.Proveedores.ToListAsync(), "ProveedorId", "Nombre");
                ViewData["Productos"] = await _context.Productos.Include(p => p.Categoria).ToListAsync();
                return View();
            }
        }

        // GET: Compras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.Producto)
                .FirstOrDefaultAsync(c => c.CompraId == id);

            if (compra == null)
            {
                return NotFound();
            }

            ViewData["Proveedores"] = new SelectList(
                await _context.Proveedores.ToListAsync(), 
                "ProveedorId", 
                "Nombre", 
                compra.ProveedorId);

            return View(compra);
        }

        // POST: Compras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompraId,ProveedorId,UsuarioId,Fecha,Total,Estado")] Compra compra)
        {
            if (id != compra.CompraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compra);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Compra actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompraExists(compra.CompraId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            
            ViewData["Proveedores"] = new SelectList(
                await _context.Proveedores.ToListAsync(), 
                "ProveedorId", 
                "Nombre", 
                compra.ProveedorId);
                
            return View(compra);
        }

        // POST: Cambiar estado de compra
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int compraId, string nuevoEstado)
        {
            var compra = await _context.Compras.FindAsync(compraId);
            if (compra == null)
            {
                return Json(new { success = false, message = "Compra no encontrada" });
            }

            compra.Estado = nuevoEstado;

            // Si la compra se completa, actualizar stock y precios con margen de ganancia
            if (nuevoEstado == "Completada")
            {
                var detalles = await _context.DetalleCompras
                    .Include(dc => dc.Producto)
                    .Where(dc => dc.CompraId == compraId)
                    .ToListAsync();

                // Obtener margen de ganancia configurado
                var margenGanancia = await ObtenerMargenGanancia();
                var productosActualizados = new List<string>();

                foreach (var detalle in detalles)
                {
                    // Actualizar stock
                    detalle.Producto.Stock += detalle.Cantidad;
                    
                    // Comentado temporalmente hasta que se agregue la columna PrecioCompra
                    // detalle.Producto.PrecioCompra = detalle.PrecioUnitario;
                    
                    // Calcular nuevo precio de venta con margen de ganancia
                    var nuevoPrecioVenta = detalle.PrecioUnitario + (detalle.PrecioUnitario * margenGanancia / 100);
                    detalle.Producto.Precio = Math.Round(nuevoPrecioVenta, 0); // Redondear a peso entero
                    
                    // Actualizar fecha de modificación
                    detalle.Producto.UpdatedAt = DateTime.Now;
                    
                    productosActualizados.Add($"{detalle.Producto.Nombre}: ${detalle.PrecioUnitario:N0} → ${detalle.Producto.Precio:N0}");
                }

                await _context.SaveChangesAsync();

                var mensaje = $"Compra completada exitosamente. Stock actualizado y precios recalculados con margen del {margenGanancia}%. " +
                             $"Productos actualizados: {productosActualizados.Count}";
                
                return Json(new { 
                    success = true, 
                    message = mensaje,
                    margenAplicado = margenGanancia,
                    productosActualizados = productosActualizados
                });
            }
            else
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"Estado cambiado a {nuevoEstado}" });
            }
        }

        private async Task<decimal> ObtenerMargenGanancia()
        {
            try
            {
                var configuracion = await _context.ConfiguracionSistema
                    .FirstOrDefaultAsync(c => c.Clave == "MARGEN_GANANCIA");

                if (configuracion != null && decimal.TryParse(configuracion.Valor, out decimal margen))
                {
                    return margen;
                }

                // Si no existe configuración, crear una con valor por defecto del 20%
                var nuevaConfiguracion = new ConfiguracionSistema
                {
                    Clave = "MARGEN_GANANCIA",
                    Valor = "20",
                    Descripcion = "Porcentaje de ganancia aplicado automáticamente a los precios de venta",
                    CreatedAt = DateTime.Now
                };

                _context.ConfiguracionSistema.Add(nuevaConfiguracion);
                await _context.SaveChangesAsync();

                return 20m;
            }
            catch (Exception)
            {
                // Si hay error con la base de datos, usar valor por defecto
                return 20m; // Valor por defecto 20%
            }
        }

        // GET: Compras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.Producto)
                .FirstOrDefaultAsync(m => m.CompraId == id);

            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // POST: Compras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.DetalleCompras)
                .FirstOrDefaultAsync(c => c.CompraId == id);

            if (compra != null)
            {
                // Eliminar primero los detalles
                _context.DetalleCompras.RemoveRange(compra.DetalleCompras);
                // Luego eliminar la compra
                _context.Compras.Remove(compra);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Compra eliminada exitosamente";
            return RedirectToAction(nameof(Index));
        }

        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.CompraId == id);
        }
    }
}
