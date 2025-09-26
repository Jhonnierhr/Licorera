using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Licorera.Services;
using Microsoft.Extensions.Options;
using Licorera.Models;

namespace Licorera.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmailTestController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailTestController> _logger;
        private readonly EmailSettings _emailSettings;
        private readonly AdminNotificationSettings _adminSettings;

        public EmailTestController(
            IEmailService emailService, 
            ILogger<EmailTestController> logger,
            IOptions<EmailSettings> emailSettings,
            IOptions<AdminNotificationSettings> adminSettings)
        {
            _emailService = emailService;
            _logger = logger;
            _emailSettings = emailSettings.Value;
            _adminSettings = adminSettings.Value;
        }

        public IActionResult Index()
        {
            // Verificar configuración
            ViewBag.EmailConfigured = !string.IsNullOrEmpty(_emailSettings.Password) && 
                                     !_emailSettings.Password.Contains("REEMPLAZAR") &&
                                     !_emailSettings.Password.Contains("tu-contraseña");
            ViewBag.SenderEmail = _emailSettings.SenderEmail;
            ViewBag.AdminEmail = _adminSettings.Email;
            ViewBag.CurrentPassword = _emailSettings.Password?.Substring(0, 4) + "****"; // Mostrar solo primeros 4 caracteres
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendTestEmail()
        {
            try
            {
                await _emailService.SendNewUserNotificationToAdminAsync(
                    "Cliente de Prueba",
                    "test@example.com",
                    "+57 300 123 4567",
                    "Carrera 10 #20-30, Bogotá"
                );

                TempData["SuccessMessage"] = "? Email de notificación al administrador enviado exitosamente. Revisa tu bandeja de entrada.";
                _logger.LogInformation("Email de prueba al administrador enviado exitosamente");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"? Error al enviar el email de administrador: {ex.Message}";
                _logger.LogError(ex, "Error al enviar email de prueba al administrador");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SendWelcomeTestEmail()
        {
            try
            {
                // Enviar al email del administrador para poder verificar
                await _emailService.SendWelcomeEmailToClientAsync(
                    "Juan Pérez García",
                    "jhonnierhr08@gmail.com" // Enviar al mismo email para poder verificar
                );

                TempData["SuccessMessage"] = "? Email de bienvenida al cliente enviado exitosamente. Revisa tu bandeja de entrada en jhonnierhr08@gmail.com.";
                _logger.LogInformation("Email de bienvenida de prueba enviado exitosamente");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"? Error al enviar el email de bienvenida: {ex.Message}";
                _logger.LogError(ex, "Error al enviar email de bienvenida de prueba: {ExceptionDetails}", ex.ToString());
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TestDirectEmailToClient()
        {
            try
            {
                // Prueba directa enviando a un email específico
                var testEmail = Request.Form["testEmail"].ToString();
                if (string.IsNullOrEmpty(testEmail))
                {
                    testEmail = "jhonnierhr08@gmail.com"; // Fallback
                }

                await _emailService.SendWelcomeEmailToClientAsync(
                    "Cliente de Prueba Directa",
                    testEmail
                );

                TempData["SuccessMessage"] = $"? Email de prueba enviado directamente a {testEmail}. Revisa la bandeja de entrada.";
                _logger.LogInformation($"Email de prueba directo enviado a {testEmail}");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"? Error en prueba directa: {ex.Message}";
                _logger.LogError(ex, "Error en prueba directa de email");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult DiagnosticInfo()
        {
            var diagnosticInfo = new
            {
                EmailSettings = new
                {
                    SmtpServer = _emailSettings.SmtpServer,
                    SmtpPort = _emailSettings.SmtpPort,
                    SenderEmail = _emailSettings.SenderEmail,
                    SenderName = _emailSettings.SenderName,
                    EnableSsl = _emailSettings.EnableSsl,
                    PasswordConfigured = !string.IsNullOrEmpty(_emailSettings.Password) && _emailSettings.Password.Length > 10,
                    PasswordLength = _emailSettings.Password?.Length ?? 0
                },
                AdminSettings = new
                {
                    Email = _adminSettings.Email,
                    Name = _adminSettings.Name
                }
            };

            return Json(diagnosticInfo);
        }
    }
}