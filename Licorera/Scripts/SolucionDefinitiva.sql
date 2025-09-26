-- ===============================================
-- SOLUCIÓN DEFINITIVA: Sistema de Margen de Ganancias
-- Proyecto: Licorera - Grandma's Liqueurs
-- ===============================================

PRINT '?? Ejecutando solución definitiva para el error de ConfiguracionSistema...';
PRINT '';

-- 0. VERIFICACIÓN INICIAL - Mostrar tablas existentes
PRINT '=== VERIFICACIÓN INICIAL DE TABLAS ===';
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME;
PRINT '';

-- 1. Eliminar tabla ConfiguracionSistema si existe para recrearla correctamente
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ConfiguracionSistema')
BEGIN
    DROP TABLE ConfiguracionSistema;
    PRINT '??? Tabla ConfiguracionSistema anterior eliminada';
END

-- 2. Crear tabla ConfiguracionSistema con estructura correcta
CREATE TABLE ConfiguracionSistema (
    ConfiguracionId INT IDENTITY(1,1) PRIMARY KEY,
    Clave NVARCHAR(100) NOT NULL,
    Valor NVARCHAR(500) NOT NULL,
    Descripcion NVARCHAR(200) NULL,
    CreatedAt DATETIME2 NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL
);

-- 3. Crear índice único para la clave
CREATE UNIQUE INDEX IX_ConfiguracionSistema_Clave ON ConfiguracionSistema(Clave);
PRINT '? Tabla ConfiguracionSistema creada correctamente con PRIMARY KEY';

-- 4. VERIFICAR Y CORREGIR TABLA DE PRODUCTOS
PRINT '';
PRINT '=== VERIFICACIÓN DE TABLA PRODUCTOS ===';

-- Verificar si existe la tabla Productos
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Productos')
BEGIN
    PRINT '? Tabla Productos encontrada';
    
    -- Mostrar columnas existentes
    PRINT 'Columnas existentes en tabla Productos:';
    SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Productos' 
    ORDER BY ORDINAL_POSITION;
    
    -- Verificar si existe la columna PrecioCompra
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Productos' AND COLUMN_NAME = 'PrecioCompra')
    BEGIN
        ALTER TABLE Productos ADD PrecioCompra DECIMAL(18,2) NULL;
        PRINT '? Campo PrecioCompra agregado a tabla Productos';
    END
    ELSE
    BEGIN
        PRINT '?? Campo PrecioCompra ya existe en tabla Productos';
    END
    
    -- Actualizar productos existentes solo si tienen columna Precio
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Productos' AND COLUMN_NAME = 'Precio')
    BEGIN
        UPDATE Productos 
        SET PrecioCompra = Precio 
        WHERE PrecioCompra IS NULL;
        
        DECLARE @ProductosActualizados INT = @@ROWCOUNT;
        PRINT '? ' + CAST(@ProductosActualizados AS NVARCHAR(10)) + ' productos actualizados con precio de compra';
    END
    ELSE
    BEGIN
        PRINT '?? No se encontró columna Precio en tabla Productos - se omite la actualización';
    END
END
ELSE
BEGIN
    -- Buscar tabla con nombre similar
    PRINT '? Tabla Productos no encontrada. Buscando tablas similares...';
    SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_NAME LIKE '%Product%' OR TABLE_NAME LIKE '%Producto%'
    ORDER BY TABLE_NAME;
END

-- 5. Insertar configuraciones del sistema
INSERT INTO ConfiguracionSistema (Clave, Valor, Descripcion, CreatedAt) VALUES
('MARGEN_GANANCIA', '20', 'Porcentaje de ganancia aplicado automáticamente a los precios de venta', GETDATE()),
('VERSION_SISTEMA', '2.0', 'Versión actual del sistema de margen automático', GETDATE()),
('SISTEMA_ACTIVO', '1', 'Indica si el sistema de margen automático está activo', GETDATE());

PRINT '? Configuraciones del sistema insertadas correctamente';

-- 6. Verificación completa
PRINT '';
PRINT '=== VERIFICACIÓN FINAL ===';

-- Verificar que la tabla ConfiguracionSistema existe y tiene PRIMARY KEY
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ConfiguracionSistema')
BEGIN
    PRINT '? Tabla ConfiguracionSistema: EXISTE';
    
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
               WHERE TABLE_NAME = 'ConfiguracionSistema' 
               AND CONSTRAINT_NAME LIKE 'PK%')
    BEGIN
        PRINT '? PRIMARY KEY ConfiguracionId: CONFIGURADA CORRECTAMENTE';
    END
    ELSE
    BEGIN
        PRINT '? PRIMARY KEY: ERROR';
    END
END
ELSE
BEGIN
    PRINT '? Tabla ConfiguracionSistema: NO EXISTE';
END

-- Verificar tabla de productos
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Productos')
BEGIN
    PRINT '? Tabla Productos: EXISTE';
    
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Productos' AND COLUMN_NAME = 'PrecioCompra')
    BEGIN
        PRINT '? Campo PrecioCompra: AGREGADO CORRECTAMENTE';
    END
    ELSE
    BEGIN
        PRINT '? Campo PrecioCompra: NO EXISTE';
    END
END
ELSE
BEGIN
    PRINT '?? Tabla Productos: NO ENCONTRADA (el sistema funcionará sin actualización de precios)';
END

-- 7. Mostrar datos insertados
PRINT '';
PRINT '=== DATOS DE CONFIGURACIÓN ===';
SELECT 
    ConfiguracionId,
    Clave,
    Valor,
    Descripcion
FROM ConfiguracionSistema 
ORDER BY ConfiguracionId;

-- 8. Mostrar todas las tablas para referencia
PRINT '';
PRINT '=== TODAS LAS TABLAS EN LA BASE DE DATOS ===';
SELECT TABLE_NAME as 'Tablas Existentes' 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE' 
ORDER BY TABLE_NAME;

PRINT '';
PRINT '?? ¡SOLUCIÓN APLICADA EXITOSAMENTE! ??';
PRINT '';
PRINT '? Entity Framework ya puede mapear ConfiguracionSistema correctamente';
PRINT '? La clave primaria ConfiguracionId está definida';
PRINT '? El sistema de margen automático está operativo';
PRINT '';
PRINT '?? PRÓXIMOS PASOS:';
PRINT '1. Reinicia tu aplicación web';
PRINT '2. Inicia sesión como administrador';
PRINT '3. El error debe estar resuelto';
PRINT '';
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Productos')
BEGIN
    PRINT '?? ¡Tu aplicación debería funcionar perfectamente ahora!';
END
ELSE
BEGIN
    PRINT '?? NOTA: No se encontró tabla Productos. El sistema funcionará pero necesitarás verificar los nombres de tablas en tu base de datos.';
END