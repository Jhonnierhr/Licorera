using System.Diagnostics;
using Licorera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Licorera.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GestionNegocioContext _context;

        public HomeController(ILogger<HomeController> logger, GestionNegocioContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Get user statistics based on role
                if (User.IsInRole("Admin"))
                {
                    ViewBag.TotalUsuarios = await _context.Usuarios.CountAsync();
                    ViewBag.TotalClientes = await _context.Clientes.CountAsync();
                    ViewBag.TotalProductos = await _context.Productos.CountAsync();
                    ViewBag.TotalVentas = await _context.Ventas.CountAsync();
                    ViewBag.VentasHoy = await _context.Ventas
                        .Where(v => v.Fecha.HasValue && v.Fecha.Value.Date == DateTime.Today)
                        .CountAsync();
                    ViewBag.IngresosHoy = await _context.Ventas
                        .Where(v => v.Fecha.HasValue && v.Fecha.Value.Date == DateTime.Today)
                        .SumAsync(v => v.Total);
                }
                else if (User.IsInRole("Cliente"))
                {
                    var userEmail = User.Identity.Name;
                    var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                    if (cliente != null)
                    {
                        ViewBag.TotalPedidos = await _context.Pedidos
                            .Where(p => p.ClienteId == cliente.ClienteId)
                            .CountAsync();
                        ViewBag.PedidosPendientes = await _context.Pedidos
                            .Where(p => p.ClienteId == cliente.ClienteId && p.Estado == "Pendiente")
                            .CountAsync();
                    }
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
