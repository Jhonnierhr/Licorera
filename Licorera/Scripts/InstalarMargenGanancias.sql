-- ===============================================
-- Script de Migraci�n: Sistema de Margen de Ganancias Autom�tico
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

-- 3. Insertar configuraci�n por defecto del margen de ganancia
IF NOT EXISTS (SELECT * FROM ConfiguracionSistema WHERE Clave = 'MARGEN_GANANCIA')
BEGIN
    INSERT INTO ConfiguracionSistema (Clave, Valor, Descripcion, CreatedAt)
    VALUES ('MARGEN_GANANCIA', '20', 'Porcentaje de ganancia aplicado autom�ticamente a los precios de venta', GETDATE());
    PRINT '? Configuraci�n de margen de ganancia creada con valor por defecto del 20%';
END
ELSE
BEGIN
    PRINT '?? Configuraci�n de margen de ganancia ya existe';
END

-- 4. Actualizar productos existentes con precio de compra igual al precio actual (opcional)
-- Solo si el precio de compra es NULL
UPDATE Productos 
SET PrecioCompra = Precio 
WHERE PrecioCompra IS NULL;

DECLARE @ProductosActualizados INT = @@ROWCOUNT;
PRINT '? ' + CAST(@ProductosActualizados AS NVARCHAR(10)) + ' productos actualizados con precio de compra';

-- 5. Verificar la instalaci�n
PRINT '';
PRINT '=== VERIFICACI�N DE INSTALACI�N ===';

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

-- Verificar configuraci�n de margen
IF EXISTS (SELECT * FROM ConfiguracionSistema WHERE Clave = 'MARGEN_GANANCIA')
    PRINT '? Configuraci�n Margen: OK';
ELSE
    PRINT '? Configuraci�n Margen: ERROR';

-- Mostrar estad�sticas
DECLARE @TotalConfiguraciones INT = (SELECT COUNT(*) FROM ConfiguracionSistema);
DECLARE @TotalProductos INT = (SELECT COUNT(*) FROM Productos);
DECLARE @ProductosConPrecioCompra INT = (SELECT COUNT(*) FROM Productos WHERE PrecioCompra IS NOT NULL);

PRINT '';
PRINT '=== ESTAD�STICAS ===';
PRINT 'Total configuraciones: ' + CAST(@TotalConfiguraciones AS NVARCHAR(10));
PRINT 'Total productos: ' + CAST(@TotalProductos AS NVARCHAR(10));
PRINT 'Productos con precio de compra: ' + CAST(@ProductosConPrecioCompra AS NVARCHAR(10));

-- Mostrar configuraci�n actual
SELECT 
    Clave as 'Configuraci�n',
    Valor as 'Valor Actual',
    Descripcion as 'Descripci�n',
    CreatedAt as 'Fecha Creaci�n'
FROM ConfiguracionSistema 
WHERE Clave = 'MARGEN_GANANCIA';

PRINT '';
PRINT '?? �MIGRACI�N COMPLETADA EXITOSAMENTE! ??';
PRINT '';
PRINT '?? Sistema de Margen de Ganancias Autom�tico implementado';
PRINT '?? Margen por defecto configurado al 20%';
PRINT '?? Los precios se calcular�n autom�ticamente al completar compras';
PRINT '';
PRINT '?? PR�XIMOS PASOS:';
PRINT '1. Ve al men� Configuraci�n para ajustar el margen';
PRINT '2. Crea compras normalmente con precios de proveedor';
PRINT '3. Al completar compras, los precios se actualizar�n autom�ticamente';
PRINT '';
PRINT '? �Tu sistema est� listo para usar!';

-- Insertar configuraciones adicionales �tiles (opcional)
IF NOT EXISTS (SELECT * FROM ConfiguracionSistema WHERE Clave = 'VERSION_SISTEMA')
BEGIN
    INSERT INTO ConfiguracionSistema (Clave, Valor, Descripcion, CreatedAt)
    VALUES ('VERSION_SISTEMA', '1.0', 'Versi�n actual del sistema de margen autom�tico', GETDATE());
END

IF NOT EXISTS (SELECT * FROM ConfiguracionSistema WHERE Clave = 'FECHA_IMPLEMENTACION')
BEGIN
    INSERT INTO ConfiguracionSistema (Clave, Valor, Descripcion, CreatedAt)
    VALUES ('FECHA_IMPLEMENTACION', CAST(GETDATE() AS NVARCHAR(50)), 'Fecha de implementaci�n del sistema de margen autom�tico', GETDATE());
END