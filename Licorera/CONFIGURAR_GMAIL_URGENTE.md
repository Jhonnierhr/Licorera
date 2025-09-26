# ?? GU�A URGENTE: Configurar Gmail para Env�o de Emails

## ?? PROBLEMA IDENTIFICADO

El email de bienvenida NO est� llegando a los clientes porque **falta configurar la contrase�a de aplicaci�n de Gmail**.

## ?? SOLUCI�N R�PIDA (5 minutos)

### Paso 1: Configurar Gmail

1. **Ve a tu Gmail** (`jhonnierhr08@gmail.com`)
2. **Haz clic en tu foto de perfil** (esquina superior derecha)
3. **Selecciona "Administrar tu cuenta de Google"**
4. **Ve a la secci�n "Seguridad"** (en el men� lateral)

### Paso 2: Habilitar 2FA (si no lo tienes)

1. **Busca "Verificaci�n en 2 pasos"**
2. **Haz clic en "Empezar"**
3. **Sigue las instrucciones** para habilitar 2FA con tu tel�fono

### Paso 3: Crear Contrase�a de Aplicaci�n

1. **Una vez habilitado 2FA**, busca **"Contrase�as de aplicaciones"**
2. **Haz clic en "Contrase�as de aplicaciones"**
3. **En el men� desplegable:**
   - Selecciona **"Correo"**
   - Selecciona **"Otro (nombre personalizado)"**
   - Escribe: **"Licorera App"**
4. **Haz clic en "Generar"**
5. **Copia la contrase�a de 16 caracteres** (formato: `xxxx xxxx xxxx xxxx`)

### Paso 4: Configurar en la Aplicaci�n

**Reemplaza en ambos archivos:**

#### En `appsettings.json`:
```json
{
  "EmailSettings": {
    "Password": "tu-contrase�a-de-16-caracteres-aqu�"
  }
}
```

#### En `appsettings.Development.json`:
```json
{
  "EmailSettings": {
    "Password": "tu-contrase�a-de-16-caracteres-aqu�"
  }
}
```

**?? IMPORTANTE:** Usa la contrase�a de aplicaci�n, NO tu contrase�a normal de Gmail.

### Paso 5: Reiniciar la Aplicaci�n

1. **Det�n la aplicaci�n** (Ctrl+C)
2. **Inicia nuevamente** (`dotnet run`)
3. **Prueba el registro** con un email real

## ?? VERIFICAR QUE FUNCIONA

### Opci�n 1: Registro Real
1. Ve a `/Account/Register`
2. Registra una cuenta con TU email real
3. Revisa tu bandeja de entrada

### Opci�n 2: Sistema de Pruebas
1. Inicia sesi�n como administrador
2. Ve a "Prueba de Emails"
3. Haz clic en "Probar Email Bienvenida"

## ?? SI A�N NO FUNCIONA

### Revisar Logs
Ejecuta la aplicaci�n y mira los logs en la consola para errores espec�ficos.

### Verificar Carpeta Spam
Los emails podr�an llegar a la carpeta de spam inicialmente.

### Verificar Configuraci�n
- ? 2FA habilitado
- ? Contrase�a de aplicaci�n generada
- ? Contrase�a copiada correctamente (sin espacios extra)
- ? Email es `jhonnierhr08@gmail.com` en ambos archivos

## ?? CHECKLIST R�PIDO

- [ ] 2FA habilitado en Gmail
- [ ] Contrase�a de aplicaci�n generada
- [ ] Contrase�a copiada en `appsettings.json`
- [ ] Contrase�a copiada en `appsettings.Development.json`
- [ ] Aplicaci�n reiniciada
- [ ] Prueba realizada

## ?? RESULTADO ESPERADO

Despu�s de configurar correctamente:
- ? **Cliente registrado recibe email de bienvenida**
- ? **Administrador recibe notificaci�n de nuevo cliente**
- ? **Los logs muestran "Email enviado exitosamente"**

---

**�Una vez configurada la contrase�a de aplicaci�n, los emails llegar�n inmediatamente!** ??