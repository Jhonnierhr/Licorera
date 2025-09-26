-- ===============================================
-- SCRIPT BÁSICO: Solo ConfiguracionSistema
-- Proyecto: Licorera - Grandma's Liqueurs
-- ===============================================

PRINT '?? Creando solo la tabla ConfiguracionSistema...';
PRINT '';

-- 1. Mostrar todas las tablas existentes para referencia
PRINT '=== TABLAS EXISTENTES EN LA BASE DE DATOS ===';
SELECT TABLE_NAME as 'Nombre de Tabla' 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE' 
ORDER BY TABLE_NAME;
PRINT '';

-- 2. Eliminar tabla ConfiguracionSistema si existe
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ConfiguracionSistema')
BEGIN
    DROP TABLE ConfiguracionSistema;
    PRINT '??? Tabla ConfiguracionSistema anterior eliminada';
END

-- 3. Crear tabla ConfiguracionSistema
CREATE TABLE ConfiguracionSistema (
    ConfiguracionId INT IDENTITY(1,1) PRIMARY KEY,
    Clave NVARCHAR(100) NOT NULL,
    Valor NVARCHAR(500) NOT NULL,
    Descripcion NVARCHAR(200) NULL,
    CreatedAt DATETIME2 NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL
);

-- 4. Crear índice único
CREATE UNIQUE INDEX IX_ConfiguracionSistema_Clave ON ConfiguracionSistema(Clave);
PRINT '? Tabla ConfiguracionSistema creada con PRIMARY KEY correctamente';

-- 5. Insertar datos iniciales
INSERT INTO ConfiguracionSistema (Clave, Valor, Descripcion, CreatedAt) VALUES
('MARGEN_GANANCIA', '20', 'Porcentaje de ganancia aplicado automáticamente a los precios de venta', GETDATE()),
('VERSION_SISTEMA', '2.0', 'Versión actual del sistema de margen automático', GETDATE()),
('SISTEMA_ACTIVO', '1', 'Indica si el sistema de margen automático está activo', GETDATE());

PRINT '? Configuraciones insertadas correctamente';

-- 6. Verificar que todo está correcto
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ConfiguracionSistema')
    AND EXISTS (SELECT * FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = 'ConfiguracionSistema')
BEGIN
    PRINT '? VERIFICACIÓN: ConfiguracionSistema creada correctamente con PRIMARY KEY';
END
ELSE
BEGIN
    PRINT '? ERROR: Algo falló en la creación';
END

-- 7. Mostrar datos insertados
PRINT '';
PRINT '=== DATOS INSERTADOS ===';
SELECT ConfiguracionId, Clave, Valor, Descripcion 
FROM ConfiguracionSistema 
ORDER BY ConfiguracionId;

PRINT '';
PRINT '? ¡SCRIPT COMPLETADO!';
PRINT '';
PRINT '?? EL ERROR DE ENTITY FRAMEWORK DEBE ESTAR RESUELTO';
PRINT '?? Reinicia tu aplicación y prueba el login como administrador';
PRINT '';
PRINT '?? NOTA: Para el margen automático en productos, necesitaremos';
PRINT '   verificar después el nombre correcto de tu tabla de productos.';