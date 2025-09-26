# ?? Guía Completa: Crear Administrador con jhonnierhr08@gmail.com

## ?? OBJETIVO
Permitir que puedas iniciar sesión como administrador usando tu email personal `jhonnierhr08@gmail.com`.

## ?? OPCIONES DE IMPLEMENTACIÓN

### **Opción 1: Automática (Recomendada)**
1. **Reinicia la aplicación** - el Program.cs actualizado creará automáticamente el usuario
2. **Ve a login** y usa: `jhonnierhr08@gmail.com` / `Admin123!`

### **Opción 2: Usar Herramienta Web**
1. **Ve a:** `localhost:puerto/Setup`
2. **Haz clic en "Crear Administrador Personal"**
3. **Verifica el resultado** en pantalla

### **Opción 3: Script SQL Manual**
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

## ? VERIFICACIÓN

### **1. Verificar que se creó correctamente:**
- Ve a: `localhost:puerto/Account/VerifyAdmin`
- Debe mostrar ambos administradores

### **2. Probar login:**
- Email: `jhonnierhr08@gmail.com`
- Contraseña: `Admin123!`

## ?? CREDENCIALES DISPONIBLES

Después de la implementación tendrás **DOS** usuarios administradores:

| Email | Contraseña | Uso |
|-------|------------|-----|
| `admin@grandmasliqueurs.com` | `Admin123!` | Administrador del sistema |
| `jhonnierhr08@gmail.com` | `Admin123!` | Tu administrador personal |

## ?? CARACTERÍSTICAS IMPLEMENTADAS

### **Program.cs Actualizado:**
- ? Crea automáticamente ambos administradores
- ? Verifica que no existan antes de crearlos
- ? Usa el mismo hash de contraseña

### **Controlador Setup:**
- ? Herramienta web para crear administradores
- ? Listar todos los administradores existentes
- ? Interfaz amigable con resultados en JSON

### **AccountController Mejorado:**
- ? Método VerifyAdmin actualizado
- ? Muestra información de todos los administradores
- ? Instrucciones de login

## ?? SEGURIDAD

### **Para Producción:**
1. **Remover SetupController** - solo para desarrollo
2. **Cambiar contraseñas** por defecto
3. **Usar contraseñas más seguras**

### **Contraseña Actual:**
- `Admin123!` - Solo para desarrollo
- Cambiar después del primer login

## ?? RESULTADO FINAL

Después de implementar cualquier opción:
- ? Podrás usar `jhonnierhr08@gmail.com` como administrador
- ? Tendrás acceso completo al panel administrativo
- ? El sistema seguirá funcionando con el administrador original
- ? Los emails de notificación seguirán llegando a tu correo

---

**?? Recomendación:** Usa la **Opción 1** (reiniciar aplicación) - es la más simple y segura.