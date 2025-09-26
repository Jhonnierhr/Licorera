# ?? Sistema de Notificaciones por Email - Guía Completa

## ?? ¿Qué hemos implementado?

Se ha implementado un **sistema completo de notificaciones por email** que:
- ? Envía automáticamente un correo **al administrador** cuando un nuevo cliente se registra
- ? Envía automáticamente un correo **de bienvenida al cliente** cuando se registra

## ?? Componentes del Sistema

### 1. **Modelos de Configuración** (`Models/EmailSettings.cs`)
- `EmailSettings`: Configuración del servidor SMTP
- `AdminNotificationSettings`: Datos del administrador que recibe las notificaciones

### 2. **Servicio de Email** (`Services/EmailService.cs`)
- `IEmailService`: Interfaz del servicio
- `EmailService`: Implementación con MailKit
- `SendNewUserNotificationToAdminAsync`: Notificación al administrador
- `SendWelcomeEmailToClientAsync`: Email de bienvenida al cliente

### 3. **Configuración** (`appsettings.json` y `appsettings.Development.json`)
- Configuración de Gmail SMTP
- Credenciales de aplicación
- Email de destino para notificaciones

### 4. **Integración en AccountController**
- Envío automático de ambos emails al registrar un nuevo cliente
- Manejo de errores sin afectar el registro
- Logging de eventos

### 5. **Sistema de Pruebas** (`EmailTestController`)
- Controlador exclusivo para administradores
- Permite probar ambos tipos de emails sin registrar usuarios reales
- Vista con interfaz amigable para cada tipo de email

### 6. **Integración en Layout**
- Enlace "Prueba de Emails" en el menú de administrador
- Solo visible para usuarios con rol Admin

## ?? Configuración Inicial Requerida

### Paso 1: Configurar Gmail
1. **Habilitar 2FA** en tu cuenta de Gmail
2. **Generar contraseña de aplicación**:
   - Ir a [Google App Passwords](https://myaccount.google.com/apppasswords)
   - Crear nueva contraseña para "Mail" ? "Other (Custom name)" ? "Licorera App"
   - Copiar la contraseña de 16 caracteres

### Paso 2: Editar archivos de configuración

**En `appsettings.json`:**
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderName": "Grandma's Liqueurs",
    "SenderEmail": "tu-email@gmail.com",        // ? TU EMAIL
    "Password": "xxxx xxxx xxxx xxxx",          // ? CONTRASEÑA DE APLICACIÓN
    "EnableSsl": true
  },
  "AdminNotification": {
    "Email": "admin@tudominio.com",             // ? EMAIL DONDE RECIBIR NOTIFICACIONES
    "Name": "Administrador"
  }
}
```

**En `appsettings.Development.json`:**
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderName": "Grandma's Liqueurs - Desarrollo",
    "SenderEmail": "tu-email@gmail.com",        // ? TU EMAIL
    "Password": "xxxx xxxx xxxx xxxx",          // ? CONTRASEÑA DE APLICACIÓN
    "EnableSsl": true
  },
  "AdminNotification": {
    "Email": "admin@tudominio.com",             // ? EMAIL DONDE RECIBIR NOTIFICACIONES
    "Name": "Administrador - Desarrollo"
  }
}
```

## ?? Flujo del Sistema

### Registro Normal de Cliente:
1. **Cliente completa formulario** de registro
2. **Sistema valida datos** y crea cuenta
3. **Se guarda en base de datos** (Usuario + Cliente)
4. **Sistema envía automáticamente 2 emails:**
   - ?? **Al administrador**: Notificación de nuevo cliente
   - ?? **Al cliente**: Email de bienvenida
5. **Usuario recibe confirmación** de registro exitoso

### Contenido del Email al Administrador:
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

### Contenido del Email de Bienvenida al Cliente:
```
?? ¡Bienvenido a Grandma's Liqueurs!

¡Hola Juan Pérez! 

Te damos la más cordial bienvenida a Grandma's Liqueurs, 
tu nueva tienda en línea de licores premium y artesanales.

?? ¿Qué puedes hacer ahora?
?? Explora nuestro catálogo completo de licores premium
?? Agrega productos a tu carrito de compras
?? Realiza pedidos y recibe en la comodidad de tu hogar
?? Accede desde cualquier dispositivo, en cualquier momento
? Disfruta de ofertas especiales y promociones exclusivas

?? Tu email de acceso: juan@example.com
?? Usa la contraseña que configuraste al registrarte

[??? Comenzar a Comprar Ahora]
```

## ?? Sistema de Pruebas

### Acceso para Administradores:
1. **Iniciar sesión** como administrador
2. **Ir al menú lateral** ? "Prueba de Emails"
3. **Probar ambos tipos de email:**
   - **"Probar Email Administrador"**: Envía notificación al admin
   - **"Probar Email Bienvenida"**: Envía bienvenida a email de prueba

### URL Directa:
- `https://tu-dominio/EmailTest`

## ??? Características de Seguridad

### ? Implementadas:
- **Credenciales separadas** del código fuente
- **Autenticación segura** con contraseña de aplicación
- **Errores de email no afectan** el proceso de registro
- **Solo administradores** pueden acceder a las pruebas
- **Logging completo** de eventos y errores
- **Validación de SSL/TLS**

### ? Manejo de Errores:
- Si falla el envío de cualquier email, el registro del usuario continúa normalmente
- Los errores se registran en los logs
- El administrador ve mensajes informativos

## ?? Monitoring y Logs

### Eventos Registrados:
- ? **Email al administrador enviado exitosamente**
- ? **Email de bienvenida al cliente enviado exitosamente**
- ? **Error al enviar emails**
- ?? **Intentos de prueba**
- ?? **Registros de nuevos usuarios**

### Consultar Logs:
```powershell
# Ver logs en tiempo real durante desarrollo
dotnet run --environment Development
```

## ?? Solución de Problemas

### ? "Error de autenticación SMTP"
**Solución:**
- Verificar que 2FA esté habilitado
- Usar contraseña de aplicación, no contraseña normal
- Confirmar que el email sea el mismo que generó la contraseña

### ? "No llegan los emails"
**Solución:**
- Revisar carpeta de spam/correo no deseado
- Verificar email en `AdminNotification.Email`
- Verificar que el cliente haya proporcionado email correcto
- Revisar logs de la aplicación
- Probar con el sistema de pruebas

### ? "Error de conexión"
**Solución:**
- Verificar que puerto 587 no esté bloqueado
- Confirmar `EnableSsl: true`
- Probar conectividad a `smtp.gmail.com`

### ? "Errores de configuración"
**Solución:**
- Verificar que todos los campos estén completos
- Confirmar formato correcto de JSON
- Reiniciar aplicación después de cambios

## ?? Personalización

### Modificar Templates de Email:
- **Para administrador**: Editar método `SendNewUserNotificationToAdminAsync` en `Services/EmailService.cs`
- **Para cliente**: Editar método `SendWelcomeEmailToClientAsync` en `Services/EmailService.cs`

### Agregar más Notificaciones:
1. Crear nuevos métodos en `IEmailService`
2. Implementar en `EmailService`
3. Llamar desde los controladores correspondientes

### Cambiar Proveedores de Email:
- Modificar configuración SMTP en `appsettings.json`
- Ajustar parámetros según el proveedor (Outlook, SendGrid, etc.)

## ?? Próximas Mejoras Sugeridas

### ?? Funcionalidades Futuras:
- **Notificaciones de pedidos** realizados
- **Reportes diarios/semanales** por email
- **Templates personalizables** desde configuración
- **Múltiples destinatarios** para notificaciones
- **Historial de emails enviados**
- **Email de recuperación de contraseña**

### ?? Optimizaciones:
- **Cola de emails** para envío asíncrono
- **Reintentos automáticos** en caso de fallo
- **Dashboard de estadísticas** de emails
- **Configuración desde panel admin**

## ? Estado Actual

### ? Completado:
- [x] Configuración de MailKit
- [x] Servicio de email funcional
- [x] Integración en registro de usuarios
- [x] **Email de bienvenida al cliente**
- [x] **Email de notificación al administrador**
- [x] Sistema de pruebas para administradores
- [x] Manejo de errores robusto
- [x] Logging completo
- [x] Documentación detallada
- [x] Templates HTML atractivos
- [x] Integración en menú de navegación

### ?? ¡Listo para usar!
El sistema está **100% funcional** y listo para producción. Solo necesitas configurar tus credenciales de Gmail en los archivos `appsettings.json` y `appsettings.Development.json`.

---

**¿Necesitas ayuda adicional?** Consulta los logs de la aplicación o usa el sistema de pruebas integrado para verificar la funcionalidad.