using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Licorera.Models;

namespace Licorera.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class CatalogoController : Controller
    {
        private readonly GestionNegocioContext _context;

        public CatalogoController(GestionNegocioContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string categoria = null, string busqueda = null, string orden = null)
        {
            var productos = _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Stock > 0)
                .AsQueryable();

            // Filtro por categoría
            if (!string.IsNullOrEmpty(categoria) && categoria != "Todas")
            {
                productos = productos.Where(p => p.Categoria.Nombre == categoria);
            }

            // Filtro por búsqueda
            if (!string.IsNullOrEmpty(busqueda))
            {
                productos = productos.Where(p => p.Nombre.Contains(busqueda) || 
                                                p.Descripcion.Contains(busqueda));
            }

            // Ordenamiento
            productos = orden switch
            {
                "precio_asc" => productos.OrderBy(p => p.Precio),
                "precio_desc" => productos.OrderByDescending(p => p.Precio),
                "nombre" => productos.OrderBy(p => p.Nombre),
                "stock" => productos.OrderByDescending(p => p.Stock),
                _ => productos.OrderBy(p => p.Nombre)
            };

            ViewBag.Categorias = await _context.Categorias.ToListAsync();
            ViewBag.CategoriaSeleccionada = categoria;
            ViewBag.BusquedaActual = busqueda;
            ViewBag.OrdenActual = orden;

            return View(await productos.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AgregarAlCarrito(int productoId, int cantidad = 1)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null || producto.Stock < cantidad)
            {
                return Json(new { success = false, message = "Producto no disponible o stock insuficiente" });
            }

            // Aquí podrías implementar la lógica del carrito usando sesiones o base de datos
            // Por ahora, simularemos que se agregó correctamente
            return Json(new { success = true, message = "Producto agregado al carrito" });
        }
    }
}