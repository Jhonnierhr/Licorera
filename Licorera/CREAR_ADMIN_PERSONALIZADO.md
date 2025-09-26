# ?? Gu�a Completa: Crear Administrador con jhonnierhr08@gmail.com

## ?? OBJETIVO
Permitir que puedas iniciar sesi�n como administrador usando tu email personal `jhonnierhr08@gmail.com`.

## ?? OPCIONES DE IMPLEMENTACI�N

### **Opci�n 1: Autom�tica (Recomendada)**
1. **Reinicia la aplicaci�n** - el Program.cs actualizado crear� autom�ticamente el usuario
2. **Ve a login** y usa: `jhonnierhr08@gmail.com` / `Admin123!`

### **Opci�n 2: Usar Herramienta Web**
1. **Ve a:** `localhost:puerto/Setup`
2. **Haz clic en "Crear Administrador Personal"**
3. **Verifica el resultado** en pantalla

### **Opci�n 3: Script SQL Manual**
```sql
-- Ejecutar en SQL Server Management Studio
USE GestionNegocio;

DECLARE @AdminRoleId INT;
SELECT @AdminRoleId = RolId FROM Roles WHERE Nombre = 'Admin';

INSERT INTO Usuarios (Nombre, Email, PasswordHash, RolId, CreatedAt)
VALUES (
    'Jhonnier Administrator',
    'jhonnierhr08@gmail.com',
    'JAvlGEgOQzn8e10ATp0Q26kOc9COjchM/a7eP/4yhkQ=',
    @AdminRoleId,
    GETDATE()
);
```

## ? VERIFICACI�N

### **1. Verificar que se cre� correctamente:**
- Ve a: `localhost:puerto/Account/VerifyAdmin`
- Debe mostrar ambos administradores

### **2. Probar login:**
- Email: `jhonnierhr08@gmail.com`
- Contrase�a: `Admin123!`

## ?? CREDENCIALES DISPONIBLES

Despu�s de la implementaci�n tendr�s **DOS** usuarios administradores:

| Email | Contrase�a | Uso |
|-------|------------|-----|
| `admin@grandmasliqueurs.com` | `Admin123!` | Administrador del sistema |
| `jhonnierhr08@gmail.com` | `Admin123!` | Tu administrador personal |

## ?? CARACTER�STICAS IMPLEMENTADAS

### **Program.cs Actualizado:**
- ? Crea autom�ticamente ambos administradores
- ? Verifica que no existan antes de crearlos
- ? Usa el mismo hash de contrase�a

### **Controlador Setup:**
- ? Herramienta web para crear administradores
- ? Listar todos los administradores existentes
- ? Interfaz amigable con resultados en JSON

### **AccountController Mejorado:**
- ? M�todo VerifyAdmin actualizado
- ? Muestra informaci�n de todos los administradores
- ? Instrucciones de login

## ?? SEGURIDAD

### **Para Producci�n:**
1. **Remover SetupController** - solo para desarrollo
2. **Cambiar contrase�as** por defecto
3. **Usar contrase�as m�s seguras**

### **Contrase�a Actual:**
- `Admin123!` - Solo para desarrollo
- Cambiar despu�s del primer login

## ?? RESULTADO FINAL

Despu�s de implementar cualquier opci�n:
- ? Podr�s usar `jhonnierhr08@gmail.com` como administrador
- ? Tendr�s acceso completo al panel administrativo
- ? El sistema seguir� funcionando con el administrador original
- ? Los emails de notificaci�n seguir�n llegando a tu correo

---

**?? Recomendaci�n:** Usa la **Opci�n 1** (reiniciar aplicaci�n) - es la m�s simple y segura.