using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using Licorera.Models;

namespace Licorera.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody);
        Task SendNewUserNotificationToAdminAsync(string userName, string userEmail, string userPhone, string userAddress);
        Task SendWelcomeEmailToClientAsync(string clientName, string clientEmail);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly AdminNotificationSettings _adminSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            IOptions<AdminNotificationSettings> adminSettings,
            ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _adminSettings = adminSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email enviado exitosamente a {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar email a {toEmail}: {ex.Message}");
                throw;
            }
        }

        public async Task SendWelcomeEmailToClientAsync(string clientName, string clientEmail)
        {
            var subject = "?? ¡Bienvenido a Grandma's Liqueurs!";
            
            var htmlBody = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <style>
                    .container {{ 
                        max-width: 600px; 
                        margin: 0 auto; 
                        font-family: Arial, sans-serif; 
                        background-color: #f8f9fa;
                        padding: 20px;
                    }}
                    .card {{ 
                        background: white; 
                        border-radius: 15px; 
                        padding: 40px; 
                        box-shadow: 0 8px 25px rgba(0,0,0,0.1);
                        text-align: center;
                    }}
                    .header {{ 
                        color: #2c3e50; 
                        border-bottom: 3px solid #e4b94f;
                        padding-bottom: 20px;
                        margin-bottom: 30px;
                    }}
                    .welcome-title {{
                        font-size: 28px;
                        font-weight: bold;
                        color: #1a3d5c;
                        margin-bottom: 10px;
                    }}
                    .welcome-message {{
                        font-size: 18px;
                        color: #34495e;
                        line-height: 1.6;
                        margin-bottom: 30px;
                    }}
                    .benefits {{
                        background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
                        padding: 25px;
                        border-radius: 10px;
                        margin: 30px 0;
                        text-align: left;
                    }}
                    .benefit-item {{
                        display: flex;
                        align-items: center;
                        margin-bottom: 15px;
                        font-size: 16px;
                        color: #2c3e50;
                    }}
                    .benefit-icon {{
                        background: #e4b94f;
                        color: white;
                        width: 30px;
                        height: 30px;
                        border-radius: 50%;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        margin-right: 15px;
                        font-weight: bold;
                    }}
                    .cta-button {{
                        display: inline-block;
                        background: linear-gradient(135deg, #1a3d5c 0%, #2c5282 100%);
                        color: white;
                        padding: 15px 30px;
                        text-decoration: none;
                        border-radius: 25px;
                        font-weight: bold;
                        margin: 20px 0;
                        transition: transform 0.3s ease;
                    }}
                    .footer {{ 
                        text-align: center; 
                        margin-top: 40px; 
                        padding-top: 30px; 
                        border-top: 1px solid #ecf0f1;
                        color: #7f8c8d;
                        font-size: 14px;
                    }}
                    .social-links {{
                        margin: 20px 0;
                    }}
                    .social-links a {{
                        display: inline-block;
                        margin: 0 10px;
                        color: #3498db;
                        text-decoration: none;
                        font-size: 18px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='card'>
                        <div class='header'>
                            <h1>?? Grandma's Liqueurs</h1>
                        </div>
                        
                        <div class='welcome-title'>
                            ¡Hola {clientName}! ??
                        </div>
                        
                        <div class='welcome-message'>
                            Te damos la más cordial bienvenida a <strong>Grandma's Liqueurs</strong>, 
                            tu nueva tienda en línea de licores premium y artesanales.
                            <br><br>
                            Estamos emocionados de tenerte como parte de nuestra familia.
                        </div>
                        
                        <div class='benefits'>
                            <h3 style='color: #2c3e50; margin-bottom: 20px; text-align: center;'>
                                ?? ¿Qué puedes hacer ahora?
                            </h3>
                            
                            <div class='benefit-item'>
                                <div class='benefit-icon'>??</div>
                                <div>Explora nuestro catálogo completo de licores premium</div>
                            </div>
                            
                            <div class='benefit-item'>
                                <div class='benefit-icon'>??</div>
                                <div>Agrega productos a tu carrito de compras</div>
                            </div>
                            
                            <div class='benefit-item'>
                                <div class='benefit-icon'>??</div>
                                <div>Realiza pedidos y recibe en la comodidad de tu hogar</div>
                            </div>
                            
                            <div class='benefit-item'>
                                <div class='benefit-icon'>??</div>
                                <div>Accede desde cualquier dispositivo, en cualquier momento</div>
                            </div>
                            
                            <div class='benefit-item'>
                                <div class='benefit-icon'>?</div>
                                <div>Disfruta de ofertas especiales y promociones exclusivas</div>
                            </div>
                        </div>
                        
                        <div style='background: #d4edda; border: 1px solid #c3e6cb; color: #155724; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                            <strong>?? Consejo:</strong> Guarda este email para futuras referencias. 
                            Aquí tienes todos los datos importantes de tu cuenta.
                        </div>
                        
                        <div style='background: #fff3cd; border: 1px solid #ffeaa7; color: #856404; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                            <strong>?? Tu email de acceso:</strong> {clientEmail}
                            <br>
                            <strong>?? Recuerda:</strong> Usa la contraseña que configuraste al registrarte
                        </div>
                        
                        <a href='#' class='cta-button' style='color: white;'>
                            ??? Comenzar a Comprar Ahora
                        </a>
                        
                        <div class='social-links'>
                            <strong>Síguenos en:</strong><br>
                            <a href='#'>?? Facebook</a>
                            <a href='#'>?? Instagram</a>
                            <a href='#'>?? Twitter</a>
                        </div>
                        
                        <div class='footer'>
                            <p><strong>¿Necesitas ayuda?</strong></p>
                            <p>?? Teléfono: +57 123 456 7890</p>
                            <p>?? Email: info@grandmasliqueurs.com</p>
                            <p>?? Horario: Lun - Sáb: 9:00 AM - 8:00 PM</p>
                            <hr style='margin: 20px 0;'>
                            <p>Este correo fue generado automáticamente. Por favor, no respondas a este mensaje.</p>
                            <p>© {DateTime.Now.Year} Grandma's Liqueurs - Todos los derechos reservados</p>
                        </div>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(clientEmail, clientName, subject, htmlBody);
        }

        public async Task SendNewUserNotificationToAdminAsync(string userName, string userEmail, string userPhone, string userAddress)
        {
            var subject = "?? Nuevo Cliente Registrado - Grandma's Liqueurs";
            
            var htmlBody = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <style>
                    .container {{ 
                        max-width: 600px; 
                        margin: 0 auto; 
                        font-family: Arial, sans-serif; 
                        background-color: #f8f9fa;
                        padding: 20px;
                    }}
                    .card {{ 
                        background: white; 
                        border-radius: 10px; 
                        padding: 30px; 
                        box-shadow: 0 4px 6px rgba(0,0,0,0.1);
                    }}
                    .header {{ 
                        text-align: center; 
                        color: #2c3e50; 
                        border-bottom: 3px solid #e4b94f;
                        padding-bottom: 20px;
                        margin-bottom: 30px;
                    }}
                    .info-row {{ 
                        display: flex; 
                        margin-bottom: 15px; 
                        padding: 10px;
                        background-color: #f8f9fa;
                        border-radius: 5px;
                    }}
                    .info-label {{ 
                        font-weight: bold; 
                        color: #34495e; 
                        min-width: 120px;
                    }}
                    .info-value {{ 
                        color: #2c3e50; 
                        margin-left: 15px;
                    }}
                    .footer {{ 
                        text-align: center; 
                        margin-top: 30px; 
                        padding-top: 20px; 
                        border-top: 1px solid #ecf0f1;
                        color: #7f8c8d;
                        font-size: 14px;
                    }}
                    .alert {{ 
                        background-color: #d4edda; 
                        border: 1px solid #c3e6cb; 
                        color: #155724; 
                        padding: 15px; 
                        border-radius: 5px; 
                        margin-bottom: 20px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='card'>
                        <div class='header'>
                            <h1>?? Grandma's Liqueurs</h1>
                            <h2>Nuevo Cliente Registrado</h2>
                        </div>
                        
                        <div class='alert'>
                            <strong>¡Excelente noticia!</strong> Un nuevo cliente se ha registrado en la plataforma.
                        </div>
                        
                        <h3 style='color: #2c3e50; margin-bottom: 20px;'>?? Información del Cliente:</h3>
                        
                        <div class='info-row'>
                            <div class='info-label'>?? Nombre:</div>
                            <div class='info-value'>{userName}</div>
                        </div>
                        
                        <div class='info-row'>
                            <div class='info-label'>?? Email:</div>
                            <div class='info-value'>{userEmail}</div>
                        </div>
                        
                        <div class='info-row'>
                            <div class='info-label'>?? Teléfono:</div>
                            <div class='info-value'>{userPhone}</div>
                        </div>
                        
                        <div class='info-row'>
                            <div class='info-label'>?? Dirección:</div>
                            <div class='info-value'>{userAddress}</div>
                        </div>
                        
                        <div class='info-row'>
                            <div class='info-label'>?? Fecha de Registro:</div>
                            <div class='info-value'>{DateTime.Now:dd/MM/yyyy HH:mm}</div>
                        </div>
                        
                        <div class='footer'>
                            <p>Este correo fue generado automáticamente por el sistema de Grandma's Liqueurs.</p>
                            <p>Para más información, accede al panel de administración.</p>
                        </div>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(_adminSettings.Email, _adminSettings.Name, subject, htmlBody);
        }
    }
}