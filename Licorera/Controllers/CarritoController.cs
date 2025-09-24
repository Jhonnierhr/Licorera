using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Licorera.Models;
using Licorera.Helpers;
using System.Security.Claims;

namespace Licorera.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class CarritoController : Controller
    {
        private readonly GestionNegocioContext _context;
        private const string CARRITO_SESSION_KEY = "Carrito";

        public CarritoController(GestionNegocioContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var carrito = HttpContext.Session.GetObjectFromJson<Carrito>(CARRITO_SESSION_KEY) ?? new Carrito();
            
            // Actualizar precios y stock de los productos en el carrito
            await ActualizarCarritoConDatosActuales(carrito);
            
            return View(carrito);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(int productoId, int cantidad = 1)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.ProductoId == productoId);

            if (producto == null)
            {
                return Json(new { success = false, message = "Producto no encontrado" });
            }

            if (producto.Stock < cantidad)
            {
                return Json(new { success = false, message = "Stock insuficiente. Disponible: " + producto.Stock });
            }

            var carrito = HttpContext.Session.GetObjectFromJson<Carrito>(CARRITO_SESSION_KEY) ?? new Carrito();

            // Verificar si el producto ya está en el carrito
            var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            var cantidadTotal = cantidad + (itemExistente?.Cantidad ?? 0);

            if (cantidadTotal > producto.Stock)
            {
                return Json(new { success = false, message = $"No puedes agregar {cantidad} unidades. Stock disponible: {producto.Stock - (itemExistente?.Cantidad ?? 0)}" });
            }

            var carritoItem = new CarritoItem
            {
                ProductoId = producto.ProductoId,
                NombreProducto = producto.Nombre,
                Descripcion = producto.Descripcion,
                Categoria = producto.Categoria?.Nombre,
                Precio = producto.Precio,
                Cantidad = cantidad,
                StockDisponible = producto.Stock
            };

            carrito.AgregarItem(carritoItem);
            HttpContext.Session.SetObjectAsJson(CARRITO_SESSION_KEY, carrito);

            return Json(new 
            { 
                success = true, 
                message = $"{producto.Nombre} agregado al carrito",
                totalItems = carrito.TotalItems,
                totalProductos = carrito.TotalProductos,
                total = carrito.Total
            });
        }

        [HttpPost]
        public IActionResult Eliminar(int productoId)
        {
            var carrito = HttpContext.Session.GetObjectFromJson<Carrito>(CARRITO_SESSION_KEY) ?? new Carrito();
            
            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item == null)
            {
                return Json(new { success = false, message = "Producto no encontrado en el carrito" });
            }

            carrito.EliminarItem(productoId);
            HttpContext.Session.SetObjectAsJson(CARRITO_SESSION_KEY, carrito);

            return Json(new 
            { 
                success = true, 
                message = $"{item.NombreProducto} eliminado del carrito",
                totalItems = carrito.TotalItems,
                totalProductos = carrito.TotalProductos,
                total = carrito.Total
            });
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarCantidad(int productoId, int cantidad)
        {
            if (cantidad <= 0)
            {
                // Llamar directamente a Eliminar y devolver su resultado
                return Eliminar(productoId);
            }

            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null)
            {
                return Json(new { success = false, message = "Producto no encontrado" });
            }

            if (cantidad > producto.Stock)
            {
                return Json(new { success = false, message = $"Stock insuficiente. Disponible: {producto.Stock}" });
            }

            var carrito = HttpContext.Session.GetObjectFromJson<Carrito>(CARRITO_SESSION_KEY) ?? new Carrito();
            carrito.ActualizarCantidad(productoId, cantidad);
            HttpContext.Session.SetObjectAsJson(CARRITO_SESSION_KEY, carrito);

            return Json(new 
            { 
                success = true, 
                message = "Cantidad actualizada",
                totalItems = carrito.TotalItems,
                totalProductos = carrito.TotalProductos,
                total = carrito.Total
            });
        }

        [HttpPost]
        public IActionResult VaciarCarrito()
        {
            HttpContext.Session.Remove(CARRITO_SESSION_KEY);
            return Json(new { success = true, message = "Carrito vaciado exitosamente" });
        }

        [HttpGet]
        public IActionResult ContadorCarrito()
        {
            var carrito = HttpContext.Session.GetObjectFromJson<Carrito>(CARRITO_SESSION_KEY) ?? new Carrito();
            return Json(new 
            { 
                totalItems = carrito.TotalItems,
                totalProductos = carrito.TotalProductos,
                total = carrito.Total
            });
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarPedido()
        {
            var carrito = HttpContext.Session.GetObjectFromJson<Carrito>(CARRITO_SESSION_KEY) ?? new Carrito();
            
            if (!carrito.Items.Any())
            {
                return Json(new { success = false, message = "El carrito está vacío" });
            }

            // Obtener cliente actual
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (cliente == null)
            {
                return Json(new { success = false, message = "Cliente no encontrado" });
            }

            // Obtener usuario sistema para registrar la venta (buscar administrador o vendedor)
            var usuarioSistema = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Rol.Nombre == "Admin" || u.Rol.Nombre == "Vendedor");

            if (usuarioSistema == null)
            {
                return Json(new { success = false, message = "No se encontró un usuario del sistema para registrar la venta" });
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Verificar stock disponible antes de procesar
                    foreach (var item in carrito.Items)
                    {
                        var producto = await _context.Productos.FindAsync(item.ProductoId);
                        if (producto == null || producto.Stock < item.Cantidad)
                        {
                            await transaction.RollbackAsync();
                            return Json(new { success = false, message = $"Stock insuficiente para {item.NombreProducto}" });
                        }
                    }

                    // Crear el pedido
                    var pedido = new Pedido
                    {
                        ClienteId = cliente.ClienteId,
                        Fecha = DateTime.Now,
                        Total = carrito.Total,
                        Estado = "Pendiente"
                    };

                    _context.Pedidos.Add(pedido);
                    await _context.SaveChangesAsync();

                    // Agregar detalles del pedido
                    foreach (var item in carrito.Items)
                    {
                        var detalle = new DetallePedido
                        {
                            PedidoId = pedido.PedidoId,
                            ProductoId = item.ProductoId,
                            Cantidad = item.Cantidad,
                            PrecioUnitario = item.Precio
                        };

                        _context.DetallePedidos.Add(detalle);
                    }

                    await _context.SaveChangesAsync();

                    // REGISTRAR AUTOMÁTICAMENTE LA VENTA
                    var venta = new Venta
                    {
                        ClienteId = cliente.ClienteId,
                        UsuarioId = usuarioSistema.UsuarioId,
                        Fecha = DateTime.Now,
                        Total = carrito.Total,
                        Estado = "Completada"
                    };

                    _context.Ventas.Add(venta);
                    await _context.SaveChangesAsync();

                    // Agregar detalles de la venta
                    foreach (var item in carrito.Items)
                    {
                        var detalleVenta = new DetalleVenta
                        {
                            VentaId = venta.VentaId,
                            ProductoId = item.ProductoId,
                            Cantidad = item.Cantidad,
                            PrecioUnitario = item.Precio
                        };

                        _context.DetalleVentas.Add(detalleVenta);
                    }

                    // Actualizar stock de productos
                    foreach (var item in carrito.Items)
                    {
                        var producto = await _context.Productos.FindAsync(item.ProductoId);
                        if (producto != null)
                        {
                            producto.Stock -= item.Cantidad;
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Vaciar el carrito
                    HttpContext.Session.Remove(CARRITO_SESSION_KEY);

                    return Json(new 
                    { 
                        success = true, 
                        message = "Pedido procesado exitosamente. La venta ha sido registrada automáticamente en el sistema.",
                        pedidoId = pedido.PedidoId,
                        ventaId = venta.VentaId
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, message = "Error al procesar el pedido: " + ex.Message });
                }
            }
        }

        private async Task ActualizarCarritoConDatosActuales(Carrito carrito)
        {
            if (!carrito.Items.Any()) return;

            var productosIds = carrito.Items.Select(i => i.ProductoId).ToList();
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => productosIds.Contains(p.ProductoId))
                .ToListAsync();

            foreach (var item in carrito.Items.ToList())
            {
                var producto = productos.FirstOrDefault(p => p.ProductoId == item.ProductoId);
                if (producto == null)
                {
                    // Producto ya no existe, remover del carrito
                    carrito.EliminarItem(item.ProductoId);
                    continue;
                }

                // Actualizar datos del producto
                item.Precio = producto.Precio;
                item.StockDisponible = producto.Stock;
                item.Categoria = producto.Categoria?.Nombre;

                // Si la cantidad en el carrito excede el stock, ajustar
                if (item.Cantidad > producto.Stock)
                {
                    item.Cantidad = producto.Stock;
                    if (item.Cantidad == 0)
                    {
                        carrito.EliminarItem(item.ProductoId);
                    }
                }
            }

            // Guardar carrito actualizado
            HttpContext.Session.SetObjectAsJson(CARRITO_SESSION_KEY, carrito);
        }
    }
}