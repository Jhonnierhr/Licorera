# Configuraci�n de Notificaciones por Email

## Paso a Paso para Configurar Gmail

### 1. Configurar tu cuenta de Gmail

1. **Habilitar autenticaci�n de 2 factores** en tu cuenta de Gmail:
   - Ve a [Google Account Security](https://myaccount.google.com/security)
   - Activa "2-Step Verification"

2. **Generar contrase�a de aplicaci�n**:
   - Ve a [App Passwords](https://myaccount.google.com/apppasswords)
   - Selecciona "Mail" y "Other (Custom name)"
   - Escribe "Licorera App"
   - Copia la contrase�a generada (16 caracteres)

### 2. Configurar appsettings.json

Edita el archivo `appsettings.json` y reemplaza los valores:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderName": "Grandma's Liqueurs",
    "SenderEmail": "tu-email@gmail.com",  // ? Tu email de Gmail
    "Password": "xxxx xxxx xxxx xxxx",    // ? Contrase�a de aplicaci�n generada
    "EnableSsl": true
  },
  "AdminNotification": {
    "Email": "admin@tudominio.com",       // ? Email donde quieres recibir las notificaciones
    "Name": "Administrador"
  }
}
```

### 3. Configuraci�n para Desarrollo

Edita `appsettings.Development.json` con los mismos datos para pruebas en desarrollo.

## �Qu� sucede cuando se registra un nuevo cliente?

1. **El usuario completa el formulario** de registro con sus datos
2. **Se crea la cuenta** en la base de datos (Usuario + Cliente)
3. **Se env�a autom�ticamente un email** al administrador con:
   - Nombre completo del cliente
   - Email del cliente
   - Tel�fono del cliente
   - Direcci�n del cliente
   - Fecha y hora del registro

## Seguridad

- ? Las credenciales de email est�n separadas del c�digo
- ? Se usa autenticaci�n segura con contrase�a de aplicaci�n
- ? Los errores de email no afectan el registro del usuario
- ? Todos los eventos se registran en los logs

## Soluci�n de Problemas

### Error de autenticaci�n SMTP
- Verifica que hayas habilitado la autenticaci�n de 2 factores
- Aseg�rate de usar una contrase�a de aplicaci�n, no tu contrase�a normal de Gmail
- Verifica que el email en `SenderEmail` sea el mismo de la cuenta que gener� la contrase�a

### No llegan los emails
- Revisa la carpeta de spam/correo no deseado
- Verifica que el email en `AdminNotification.Email` sea correcto
- Revisa los logs de la aplicaci�n para errores espec�ficos

### Errores de configuraci�n
- Aseg�rate de que todos los campos requeridos est�n completos
- Verifica que el puerto 587 no est� bloqueado por firewall
- Confirma que `EnableSsl` est� en `true`

## Ejemplo de Email que se Env�a

El administrador recibir� un email con formato HTML que incluye:

```
?? Nuevo Cliente Registrado - Grandma's Liqueurs

�Excelente noticia! Un nuevo cliente se ha registrado en la plataforma.

?? Informaci�n del Cliente:
?? Nombre: Juan P�rez
?? Email: juan@example.com
?? Tel�fono: +57 300 123 4567
?? Direcci�n: Carrera 10 #20-30, Bogot�
?? Fecha de Registro: 15/12/2024 14:30
```

## Personalizaci�n

Puedes modificar el template del email editando el m�todo `SendNewUserNotificationToAdminAsync` en `Services/EmailService.cs`.