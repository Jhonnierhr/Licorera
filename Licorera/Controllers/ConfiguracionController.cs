using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Licorera.Models;

namespace Licorera.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConfiguracionController : Controller
    {
        private readonly GestionNegocioContext _context;

        public ConfiguracionController(GestionNegocioContext context)
        {
            _context = context;
        }

        // GET: Configuracion
        public async Task<IActionResult> Index()
        {
            try
            {
                var configuraciones = await _context.ConfiguracionSistema.ToListAsync();
                
                // Si no existe la configuración de margen de ganancia, crearla con valor por defecto
                var margenGanancia = configuraciones.FirstOrDefault(c => c.Clave == "MARGEN_GANANCIA");
                if (margenGanancia == null)
                {
                    margenGanancia = new ConfiguracionSistema
                    {
                        Clave = "MARGEN_GANANCIA",
                        Valor = "20",
                        Descripcion = "Porcentaje de ganancia aplicado automáticamente a los precios de venta",
                        CreatedAt = DateTime.Now
                    };
                    
                    _context.ConfiguracionSistema.Add(margenGanancia);
                    await _context.SaveChangesAsync();
                    configuraciones.Add(margenGanancia);
                }

                return View(configuraciones);
            }
            catch (Exception ex)
            {
                // Si hay error, crear lista temporal para mostrar la interfaz
                var configuracionesTemporal = new List<ConfiguracionSistema>
                {
                    new ConfiguracionSistema
                    {
                        ConfiguracionId = 1,
                        Clave = "MARGEN_GANANCIA",
                        Valor = "20",
                        Descripcion = "Porcentaje de ganancia aplicado automáticamente a los precios de venta (Temporal - Ejecuta el script SQL)",
                        CreatedAt = DateTime.Now
                    }
                };

                ViewBag.ErrorMessage = "Error de base de datos. Ejecuta el script SolucionDefinitiva.sql";
                return View(configuracionesTemporal);
            }
        }

        // POST: Configuracion/ActualizarMargenGanancia
        [HttpPost]
        public async Task<IActionResult> ActualizarMargenGanancia(decimal porcentaje)
        {
            if (porcentaje < 0 || porcentaje > 200)
            {
                return Json(new { success = false, message = "El porcentaje debe estar entre 0 y 200" });
            }

            try
            {
                var configuracion = await _context.ConfiguracionSistema
                    .FirstOrDefaultAsync(c => c.Clave == "MARGEN_GANANCIA");

                if (configuracion == null)
                {
                    configuracion = new ConfiguracionSistema
                    {
                        Clave = "MARGEN_GANANCIA",
                        Valor = porcentaje.ToString(),
                        Descripcion = "Porcentaje de ganancia aplicado automáticamente a los precios de venta",
                        CreatedAt = DateTime.Now
                    };
                    _context.ConfiguracionSistema.Add(configuracion);
                }
                else
                {
                    configuracion.Valor = porcentaje.ToString();
                    configuracion.UpdatedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = $"Margen de ganancia actualizado a {porcentaje}%" 
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Error de base de datos. Ejecuta el script SolucionDefinitiva.sql primero." 
                });
            }
        }

        // GET: Configuracion/ObtenerMargenGanancia
        [HttpGet]
        public async Task<decimal> ObtenerMargenGanancia()
        {
            try
            {
                var configuracion = await _context.ConfiguracionSistema
                    .FirstOrDefaultAsync(c => c.Clave == "MARGEN_GANANCIA");

                if (configuracion != null && decimal.TryParse(configuracion.Valor, out decimal margen))
                {
                    return margen;
                }
            }
            catch (Exception)
            {
                // Si hay error con la base de datos, usar valor por defecto
            }

            return 20m; // Valor por defecto 20%
        }
    }
}