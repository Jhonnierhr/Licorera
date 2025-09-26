-- ===============================================
-- Script de Migración: Sistema de Margen de Ganancias Automático
-- Proyecto: Licorera - Grandma's Liqueurs
-- ===============================================

-- 1. Crear tabla ConfiguracionSistema si no existe
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ConfiguracionSistema' AND xtype='U')
BEGIN
    CREATE TABLE ConfiguracionSistema (
        ConfiguracionId INT PRIMARY KEY IDENTITY(1,1),
        Clave NVARCHAR(100) NOT NULL UNIQUE,
        Valor NVARCHAR(500) NOT NULL,
        Descripcion NVARCHAR(200) NULL,
        CreatedAt DATETIME2 NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL
    );
    PRINT '? Tabla ConfiguracionSistema creada exitosamente';
END
ELSE
BEGIN
    PRINT '?? Tabla ConfiguracionSistema ya existe';
END

-- 2. Agregar campo PrecioCompra a la tabla Productos si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Productos]') AND name = 'PrecioCompra')
BEGIN
    ALTER TABLE Productos ADD PrecioCompra DECIMAL(18,2) NULL;
    PRINT '? Campo PrecioCompra agregado a tabla Productos';
END
ELSE
BEGIN
    PRINT '?? Campo PrecioCompra ya existe en tabla Productos';
END

-- 3. Insertar configuración por defecto del margen de ganancia
IF NOT EXISTS (SELECT * FROM ConfiguracionSistema WHERE Clave = 'MARGEN_GANANCIA')
BEGIN
    INSERT INTO ConfiguracionSistema (Clave, Valor, Descripcion, CreatedAt)
    VALUES ('MARGEN_GANANCIA', '20', 'Porcentaje de ganancia aplicado automáticamente a los precios de venta', GETDATE());
    PRINT '? Configuración de margen de ganancia creada con valor por defecto del 20%';
END
ELSE
BEGIN
    PRINT '?? Configuración de margen de ganancia ya existe';
END

-- 4. Actualizar productos existentes con precio de compra igual al precio actual (opcional)
-- Solo si el precio de compra es NULL
UPDATE Productos 
SET PrecioCompra = Precio 
WHERE PrecioCompra IS NULL;

DECLARE @ProductosActualizados INT = @@ROWCOUNT;
PRINT '? ' + CAST(@ProductosActualizados AS NVARCHAR(10)) + ' productos actualizados con precio de compra';

-- 5. Verificar la instalación
PRINT '';
PRINT '=== VERIFICACIÓN DE INSTALACIÓN ===';

-- Verificar tabla ConfiguracionSistema
IF EXISTS (SELECT * FROM sysobjects WHERE name='ConfiguracionSistema' AND xtype='U')
    PRINT '? Tabla ConfiguracionSistema: OK';
ELSE
    PRINT '? Tabla ConfiguracionSistema: ERROR';

-- Verificar campo PrecioCompra
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Productos]') AND name = 'PrecioCompra')
    PRINT '? Campo PrecioCompra: OK';
ELSE
    PRINT '? Campo PrecioCompra: ERROR';

-- Verificar configuración de margen
IF EXISTS (SELECT * FROM ConfiguracionSistema WHERE Clave = 'MARGEN_GANANCIA')
    PRINT '? Configuración Margen: OK';
ELSE
    PRINT '? Configuración Margen: ERROR';

-- Mostrar estadísticas
DECLARE @TotalConfiguraciones INT = (SELECT COUNT(*) FROM ConfiguracionSistema);
DECLARE @TotalProductos INT = (SELECT COUNT(*) FROM Productos);
DECLARE @ProductosConPrecioCompra INT = (SELECT COUNT(*) FROM Productos WHERE PrecioCompra IS NOT NULL);

PRINT '';
PRINT '=== ESTADÍSTICAS ===';
PRINT 'Total configuraciones: ' + CAST(@TotalConfiguraciones AS NVARCHAR(10));
PRINT 'Total productos: ' + CAST(@TotalProductos AS NVARCHAR(10));
PRINT 'Productos con precio de compra: ' + CAST(@ProductosConPrecioCompra AS NVARCHAR(10));

-- Mostrar configuración actual
SELECT 
    Clave as 'Configuración',
    Valor as 'Valor Actual',
    Descripcion as 'Descripción',
    CreatedAt as 'Fecha Creación'
FROM ConfiguracionSistema 
WHERE Clave = 'MARGEN_GANANCIA';

PRINT '';
PRINT '?? ¡MIGRACIÓN COMPLETADA EXITOSAMENTE! ??';
PRINT '';
PRINT '?? Sistema de Margen de Ganancias Automático implementado';
PRINT '?? Margen por defecto configurado al 20%';
PRINT '?? Los precios se calcularán automáticamente al completar compras';
PRINT '';
PRINT '?? PRÓXIMOS PASOS:';
PRINT '1. Ve al menú Configuración para ajustar el margen';
PRINT '2. Crea compras normalmente con precios de proveedor';
PRINT '3. Al completar compras, los precios se actualizarán automáticamente';
PRINT '';
PRINT '? ¡Tu sistema está listo para usar!';

-- Insertar configuraciones adicionales útiles (opcional)
IF NOT EXISTS (SELECT * FROM ConfiguracionSistema WHERE Clave = 'VERSION_SISTEMA')
BEGIN
    INSERT INTO ConfiguracionSistema (Clave, Valor, Descripcion, CreatedAt)
    VALUES ('VERSION_SISTEMA', '1.0', 'Versión actual del sistema de margen automático', GETDATE());
END

IF NOT EXISTS (SELECT * FROM ConfiguracionSistema WHERE Clave = 'FECHA_IMPLEMENTACION')
BEGIN
    INSERT INTO ConfiguracionSistema (Clave, Valor, Descripcion, CreatedAt)
    VALUES ('FECHA_IMPLEMENTACION', CAST(GETDATE() AS NVARCHAR(50)), 'Fecha de implementación del sistema de margen automático', GETDATE());
END