# ?? GUÍA URGENTE: Configurar Gmail para Envío de Emails

## ?? PROBLEMA IDENTIFICADO

El email de bienvenida NO está llegando a los clientes porque **falta configurar la contraseña de aplicación de Gmail**.

## ?? SOLUCIÓN RÁPIDA (5 minutos)

### Paso 1: Configurar Gmail

1. **Ve a tu Gmail** (`jhonnierhr08@gmail.com`)
2. **Haz clic en tu foto de perfil** (esquina superior derecha)
3. **Selecciona "Administrar tu cuenta de Google"**
4. **Ve a la sección "Seguridad"** (en el menú lateral)

### Paso 2: Habilitar 2FA (si no lo tienes)

1. **Busca "Verificación en 2 pasos"**
2. **Haz clic en "Empezar"**
3. **Sigue las instrucciones** para habilitar 2FA con tu teléfono

### Paso 3: Crear Contraseña de Aplicación

1. **Una vez habilitado 2FA**, busca **"Contraseñas de aplicaciones"**
2. **Haz clic en "Contraseñas de aplicaciones"**
3. **En el menú desplegable:**
   - Selecciona **"Correo"**
   - Selecciona **"Otro (nombre personalizado)"**
   - Escribe: **"Licorera App"**
4. **Haz clic en "Generar"**
5. **Copia la contraseña de 16 caracteres** (formato: `xxxx xxxx xxxx xxxx`)

### Paso 4: Configurar en la Aplicación

**Reemplaza en ambos archivos:**

#### En `appsettings.json`:
```json
{
  "EmailSettings": {
    "Password": "tu-contraseña-de-16-caracteres-aquí"
  }
}
```

#### En `appsettings.Development.json`:
```json
{
  "EmailSettings": {
    "Password": "tu-contraseña-de-16-caracteres-aquí"
  }
}
```

**?? IMPORTANTE:** Usa la contraseña de aplicación, NO tu contraseña normal de Gmail.

### Paso 5: Reiniciar la Aplicación

1. **Detén la aplicación** (Ctrl+C)
2. **Inicia nuevamente** (`dotnet run`)
3. **Prueba el registro** con un email real

## ?? VERIFICAR QUE FUNCIONA

### Opción 1: Registro Real
1. Ve a `/Account/Register`
2. Registra una cuenta con TU email real
3. Revisa tu bandeja de entrada

### Opción 2: Sistema de Pruebas
1. Inicia sesión como administrador
2. Ve a "Prueba de Emails"
3. Haz clic en "Probar Email Bienvenida"

## ?? SI AÚN NO FUNCIONA

### Revisar Logs
Ejecuta la aplicación y mira los logs en la consola para errores específicos.

### Verificar Carpeta Spam
Los emails podrían llegar a la carpeta de spam inicialmente.

### Verificar Configuración
- ? 2FA habilitado
- ? Contraseña de aplicación generada
- ? Contraseña copiada correctamente (sin espacios extra)
- ? Email es `jhonnierhr08@gmail.com` en ambos archivos

## ?? CHECKLIST RÁPIDO

- [ ] 2FA habilitado en Gmail
- [ ] Contraseña de aplicación generada
- [ ] Contraseña copiada en `appsettings.json`
- [ ] Contraseña copiada en `appsettings.Development.json`
- [ ] Aplicación reiniciada
- [ ] Prueba realizada

## ?? RESULTADO ESPERADO

Después de configurar correctamente:
- ? **Cliente registrado recibe email de bienvenida**
- ? **Administrador recibe notificación de nuevo cliente**
- ? **Los logs muestran "Email enviado exitosamente"**

---

**¡Una vez configurada la contraseña de aplicación, los emails llegarán inmediatamente!** ??