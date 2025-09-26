# Configuración de Notificaciones por Email

## Paso a Paso para Configurar Gmail

### 1. Configurar tu cuenta de Gmail

1. **Habilitar autenticación de 2 factores** en tu cuenta de Gmail:
   - Ve a [Google Account Security](https://myaccount.google.com/security)
   - Activa "2-Step Verification"

2. **Generar contraseña de aplicación**:
   - Ve a [App Passwords](https://myaccount.google.com/apppasswords)
   - Selecciona "Mail" y "Other (Custom name)"
   - Escribe "Licorera App"
   - Copia la contraseña generada (16 caracteres)

### 2. Configurar appsettings.json

Edita el archivo `appsettings.json` y reemplaza los valores:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderName": "Grandma's Liqueurs",
    "SenderEmail": "tu-email@gmail.com",  // ? Tu email de Gmail
    "Password": "xxxx xxxx xxxx xxxx",    // ? Contraseña de aplicación generada
    "EnableSsl": true
  },
  "AdminNotification": {
    "Email": "admin@tudominio.com",       // ? Email donde quieres recibir las notificaciones
    "Name": "Administrador"
  }
}
```

### 3. Configuración para Desarrollo

Edita `appsettings.Development.json` con los mismos datos para pruebas en desarrollo.

## ¿Qué sucede cuando se registra un nuevo cliente?

1. **El usuario completa el formulario** de registro con sus datos
2. **Se crea la cuenta** en la base de datos (Usuario + Cliente)
3. **Se envía automáticamente un email** al administrador con:
   - Nombre completo del cliente
   - Email del cliente
   - Teléfono del cliente
   - Dirección del cliente
   - Fecha y hora del registro

## Seguridad

- ? Las credenciales de email están separadas del código
- ? Se usa autenticación segura con contraseña de aplicación
- ? Los errores de email no afectan el registro del usuario
- ? Todos los eventos se registran en los logs

## Solución de Problemas

### Error de autenticación SMTP
- Verifica que hayas habilitado la autenticación de 2 factores
- Asegúrate de usar una contraseña de aplicación, no tu contraseña normal de Gmail
- Verifica que el email en `SenderEmail` sea el mismo de la cuenta que generó la contraseña

### No llegan los emails
- Revisa la carpeta de spam/correo no deseado
- Verifica que el email en `AdminNotification.Email` sea correcto
- Revisa los logs de la aplicación para errores específicos

### Errores de configuración
- Asegúrate de que todos los campos requeridos estén completos
- Verifica que el puerto 587 no esté bloqueado por firewall
- Confirma que `EnableSsl` esté en `true`

## Ejemplo de Email que se Envía

El administrador recibirá un email con formato HTML que incluye:

```
?? Nuevo Cliente Registrado - Grandma's Liqueurs

¡Excelente noticia! Un nuevo cliente se ha registrado en la plataforma.

?? Información del Cliente:
?? Nombre: Juan Pérez
?? Email: juan@example.com
?? Teléfono: +57 300 123 4567
?? Dirección: Carrera 10 #20-30, Bogotá
?? Fecha de Registro: 15/12/2024 14:30
```

## Personalización

Puedes modificar el template del email editando el método `SendNewUserNotificationToAdminAsync` en `Services/EmailService.cs`.