# ?? Gu�a para Solucionar Error de Tabla Productos

## ? **Error Identificado**
```
Nombre de objeto 'Productos' no v�lido.
Nombre de columna 'PrecioCompra' no v�lido.
Nombre de columna 'Precio' no v�lido.
```

## ?? **Causa del Problema**
La base de datos no tiene una tabla llamada `Productos` o las columnas no existen con esos nombres exactos.

## ? **Soluci�n Paso a Paso**

### **Opci�n 1: Script B�sico (RECOMENDADO)**
**Ejecuta primero este script para resolver el error principal:**

1. Ejecuta: `Scripts/SoloConfiguracion.sql`
2. Este script:
   - ? Crea solo la tabla `ConfiguracionSistema`
   - ? Resuelve el error de Entity Framework
   - ? Muestra todas las tablas de tu base de datos
   - ? No toca la tabla de productos

### **Opci�n 2: Script Completo con Verificaci�n**
**Si quieres intentar el script completo:**

1. Ejecuta: `Scripts/SolucionDefinitiva.sql` (versi�n actualizada)
2. Este script:
   - ? Verifica qu� tablas existen realmente
   - ? Muestra la estructura de la tabla de productos si existe
   - ? Solo modifica lo que es seguro

## ?? **Para Identificar el Problema**

Despu�s de ejecutar cualquiera de los scripts, ver�s:

### **Tablas que deber�an existir:**
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

### **Paso 1: Ejecutar Script B�sico**
```sql
-- Ejecuta: Scripts/SoloConfiguracion.sql
-- Esto resuelve el error principal de ConfiguracionSistema
```

### **Paso 2: Probar la Aplicaci�n**
1. Reinicia tu aplicaci�n
2. Intenta iniciar sesi�n como administrador
3. El error de Entity Framework deber�a estar resuelto

### **Paso 3: Identificar Estructura Real**
Despu�s de ejecutar el script, revisa la salida para ver:
- �C�mo se llama realmente tu tabla de productos?
- �Qu� columnas tiene?

### **Paso 4: Ajustar Sistema de Margen (Opcional)**
Una vez que sepamos la estructura real, podemos:
1. Agregar el campo `PrecioCompra` si no existe
2. Configurar el sistema de margen autom�tico

## ?? **Scripts Disponibles**

| Script | Prop�sito | Recomendaci�n |
|--------|-----------|---------------|
| `SoloConfiguracion.sql` | Solo arregla Entity Framework | ? **EMPEZAR AQU�** |
| `SolucionDefinitiva.sql` | Completo con verificaciones | Para despu�s |

## ?? **Resultado Esperado**

Despu�s del script b�sico:
- ? **Error de Entity Framework resuelto**
- ? **Puedes iniciar sesi�n como administrador**
- ? **Sistema de configuraci�n funciona**
- ?? **Margen autom�tico temporalmente deshabilitado hasta verificar tabla productos**

---

## ?? **Si Necesitas Ayuda**

1. **Ejecuta el script b�sico primero**
2. **Comparte la salida** del script (qu� tablas muestra)
3. **Podremos ajustar** el c�digo para tu estructura espec�fica

**�Prioridad: Resolver el error de Entity Framework primero!** ??