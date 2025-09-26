# ?? Guía para Solucionar Error de Tabla Productos

## ? **Error Identificado**
```
Nombre de objeto 'Productos' no válido.
Nombre de columna 'PrecioCompra' no válido.
Nombre de columna 'Precio' no válido.
```

## ?? **Causa del Problema**
La base de datos no tiene una tabla llamada `Productos` o las columnas no existen con esos nombres exactos.

## ? **Solución Paso a Paso**

### **Opción 1: Script Básico (RECOMENDADO)**
**Ejecuta primero este script para resolver el error principal:**

1. Ejecuta: `Scripts/SoloConfiguracion.sql`
2. Este script:
   - ? Crea solo la tabla `ConfiguracionSistema`
   - ? Resuelve el error de Entity Framework
   - ? Muestra todas las tablas de tu base de datos
   - ? No toca la tabla de productos

### **Opción 2: Script Completo con Verificación**
**Si quieres intentar el script completo:**

1. Ejecuta: `Scripts/SolucionDefinitiva.sql` (versión actualizada)
2. Este script:
   - ? Verifica qué tablas existen realmente
   - ? Muestra la estructura de la tabla de productos si existe
   - ? Solo modifica lo que es seguro

## ?? **Para Identificar el Problema**

Después de ejecutar cualquiera de los scripts, verás:

### **Tablas que deberían existir:**
- `Usuarios`
- `Roles` 
- `Clientes`
- `Productos` (o similar: `Productoes`, `Product`, etc.)
- `Categorias`
- `Compras`
- `Ventas`
- `Pedidos`

### **Si tu tabla de productos tiene otro nombre:**
Por ejemplo, si se llama `Productoes` en lugar de `Productos`, necesitaremos:

1. **Actualizar el modelo C#**:
```csharp
// En GestionNegocioContext.cs cambiar:
public virtual DbSet<Producto> Productos { get; set; }
// Por:
public virtual DbSet<Producto> Productoes { get; set; }
```

2. **Actualizar las consultas en los controladores**.

## ?? **Pasos Inmediatos**

### **Paso 1: Ejecutar Script Básico**
```sql
-- Ejecuta: Scripts/SoloConfiguracion.sql
-- Esto resuelve el error principal de ConfiguracionSistema
```

### **Paso 2: Probar la Aplicación**
1. Reinicia tu aplicación
2. Intenta iniciar sesión como administrador
3. El error de Entity Framework debería estar resuelto

### **Paso 3: Identificar Estructura Real**
Después de ejecutar el script, revisa la salida para ver:
- ¿Cómo se llama realmente tu tabla de productos?
- ¿Qué columnas tiene?

### **Paso 4: Ajustar Sistema de Margen (Opcional)**
Una vez que sepamos la estructura real, podemos:
1. Agregar el campo `PrecioCompra` si no existe
2. Configurar el sistema de margen automático

## ?? **Scripts Disponibles**

| Script | Propósito | Recomendación |
|--------|-----------|---------------|
| `SoloConfiguracion.sql` | Solo arregla Entity Framework | ? **EMPEZAR AQUÍ** |
| `SolucionDefinitiva.sql` | Completo con verificaciones | Para después |

## ?? **Resultado Esperado**

Después del script básico:
- ? **Error de Entity Framework resuelto**
- ? **Puedes iniciar sesión como administrador**
- ? **Sistema de configuración funciona**
- ?? **Margen automático temporalmente deshabilitado hasta verificar tabla productos**

---

## ?? **Si Necesitas Ayuda**

1. **Ejecuta el script básico primero**
2. **Comparte la salida** del script (qué tablas muestra)
3. **Podremos ajustar** el código para tu estructura específica

**¡Prioridad: Resolver el error de Entity Framework primero!** ??