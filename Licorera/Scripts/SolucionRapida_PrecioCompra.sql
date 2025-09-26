-- ===============================================
-- SCRIPT URGENTE: Solo agregar columna PrecioCompra
-- Proyecto: Licorera - Solución Rápida
-- ===============================================

PRINT '?? Ejecutando solución rápida para error PrecioCompra...';
PRINT '';

-- 1. Mostrar tablas existentes para verificar estructura
PRINT '=== VERIFICANDO ESTRUCTURA ACTUAL ===';
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME;
PRINT '';

-- 2. Buscar tabla de productos (podría tener diferente nombre)
DECLARE @TablaProductos NVARCHAR(128) = NULL;

-- Verificar nombres posibles para la tabla de productos
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Productos')
    SET @TablaProductos = 'Productos';
ELSE IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Productoes')
    SET @TablaProductos = 'Productoes';
ELSE IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Product')
    SET @TablaProductos = 'Product';

-- 3. Agregar columna PrecioCompra si se encontró la tabla
IF @TablaProductos IS NOT NULL
BEGIN
    PRINT '? Tabla de productos encontrada: ' + @TablaProductos;
    
    -- Verificar si ya existe la columna PrecioCompra
    DECLARE @ColumnExists INT = 0;
    SELECT @ColumnExists = COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = @TablaProductos AND COLUMN_NAME = 'PrecioCompra';
    
    IF @ColumnExists = 0
    BEGIN
        -- Construir y ejecutar comando ALTER TABLE dinámicamente
        DECLARE @SQL NVARCHAR(MAX) = 'ALTER TABLE ' + QUOTENAME(@TablaProductos) + ' ADD PrecioCompra DECIMAL(18,2) NULL';
        EXEC sp_executesql @SQL;
        PRINT '? Columna PrecioCompra agregada a tabla ' + @TablaProductos;
        
        -- Actualizar valores existentes
        SET @SQL = 'UPDATE ' + QUOTENAME(@TablaProductos) + ' SET PrecioCompra = Precio WHERE PrecioCompra IS NULL';
        EXEC sp_executesql @SQL;
        PRINT '? Valores de PrecioCompra inicializados con precio actual';
    END
    ELSE
    BEGIN
        PRINT '?? Columna PrecioCompra ya existe en tabla ' + @TablaProductos;
    END
END
ELSE
BEGIN
    PRINT '? No se encontró tabla de productos con nombres: Productos, Productoes, Product';
    PRINT 'Tablas disponibles:';
    SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME;
END

-- 4. Crear tabla ConfiguracionSistema si no existe
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ConfiguracionSistema')
BEGIN
    CREATE TABLE ConfiguracionSistema (
        ConfiguracionId INT IDENTITY(1,1) PRIMARY KEY,
        Clave NVARCHAR(100) NOT NULL,
        Valor NVARCHAR(500) NOT NULL,
        Descripcion NVARCHAR(200) NULL,
        CreatedAt DATETIME2 NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL
    );
    
    CREATE UNIQUE INDEX IX_ConfiguracionSistema_Clave ON ConfiguracionSistema(Clave);
    
    INSERT INTO ConfiguracionSistema (Clave, Valor, Descripcion, CreatedAt) VALUES
    ('MARGEN_GANANCIA', '20', 'Porcentaje de ganancia aplicado automáticamente a los precios de venta', GETDATE());
    
    PRINT '? Tabla ConfiguracionSistema creada con configuración inicial';
END
ELSE
BEGIN
    PRINT '?? Tabla ConfiguracionSistema ya existe';
END

-- 5. Verificación final
PRINT '';
PRINT '=== VERIFICACIÓN FINAL ===';

IF @TablaProductos IS NOT NULL
BEGIN
    DECLARE @PrecioCompraExists INT = 0;
    SELECT @PrecioCompraExists = COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = @TablaProductos AND COLUMN_NAME = 'PrecioCompra';
    
    IF @PrecioCompraExists > 0
        PRINT '? Columna PrecioCompra: EXISTE en ' + @TablaProductos;
    ELSE
        PRINT '? Columna PrecioCompra: NO EXISTE en ' + @TablaProductos;
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ConfiguracionSistema')
    PRINT '? Tabla ConfiguracionSistema: EXISTE';
ELSE
    PRINT '? Tabla ConfiguracionSistema: NO EXISTE';

PRINT '';
PRINT '?? ¡SCRIPT COMPLETADO!';
PRINT '';
PRINT '?? PRÓXIMOS PASOS:';
PRINT '1. Descomenta el campo PrecioCompra en Producto.cs';
PRINT '2. Descomenta las configuraciones en GestionNegocioContext.cs';
PRINT '3. Descomenta las referencias en ComprasController.cs';
PRINT '4. Reinicia la aplicación';
PRINT '';
PRINT '?? El error debe estar resuelto después de estos pasos!';