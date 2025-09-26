# ?? SOLUCI�N R�PIDA: Diagn�stico de Email

## ?? PROBLEMA IDENTIFICADO

Los emails de bienvenida no est�n llegando a los clientes. Vamos a diagnosticar paso a paso.

## ?? PASOS DE DIAGN�STICO

### Paso 1: Verificar Sistema de Pruebas
1. **Inicia sesi�n como administrador:** `admin@grandmasliqueurs.com` / `Admin123!`
2. **Ve a "Prueba de Emails"** en el men� lateral
3. **Haz clic en cada bot�n de prueba:**
   - "Probar Email Administrador" 
   - "Probar Email Bienvenida"
   - "Enviar Prueba Directa"

### Paso 2: Revisar Logs en Tiempo Real
1. **Abre la consola** donde est� corriendo la aplicaci�n
2. **Busca estos mensajes:**
   - ? `"Email enviado exitosamente a [email]"`
   - ? `"Error al enviar email a [email]"`
   - ?? `"Iniciando env�o de emails para nuevo cliente"`

### Paso 3: Verificar Configuraci�n Gmail
1. **Tu configuraci�n actual:**
   ```json
   {
     "SenderEmail": "jhonnierhr08@gmail.com",
     "Password": "qlfi srcn fzec kvmb"
   }
   ```

2. **Verificaciones:**
   - ? 2FA habilitado en jhonnierhr08@gmail.com
   - ? Contrase�a de aplicaci�n v�lida
   - ? Puerto 587 no bloqueado

### Paso 4: Prueba Manual
1. **Registra un cliente de prueba** con tu propio email
2. **Revisa estas carpetas:**
   - ?? Bandeja de entrada
   - ??? Carpeta de spam
   - ?? Promociones (si usas Gmail)

## ??? SOLUCIONES COMUNES

### Problema 1: Emails van a Spam
**Soluci�n:** Revisa la carpeta de spam y marca como "No es spam"

### Problema 2: Error de Autenticaci�n
**Soluci�n:** Regenerar contrase�a de aplicaci�n
1. Ve a https://myaccount.google.com/apppasswords
2. Elimina "Licorera App" si existe
3. Crea nueva contrase�a para "Licorera App"
4. Actualiza appsettings.json

### Problema 3: Puerto Bloqueado
**Soluci�n:** Verificar firewall
```bash
telnet smtp.gmail.com 587
```

### Problema 4: Error en el C�digo
**Revisar logs para:**
- Errores de conexi�n SMTP
- Problemas de formato de email
- Excepciones no manejadas

## ?? PRUEBA R�PIDA

**Comando de prueba desde el navegador:**
1. Ve a: `localhost:puerto/EmailTest/DiagnosticInfo`
2. Verifica que todos los valores est�n correctos

## ?? CONTACTO DE EMERGENCIA

Si el problema persiste:
1. **Copia los logs** de la consola
2. **Toma screenshot** de la p�gina de pruebas
3. **Verifica** que jhonnierhr08@gmail.com reciba emails normalmente

---

**?? Meta: Los clientes deben recibir emails de bienvenida inmediatamente despu�s del registro.**