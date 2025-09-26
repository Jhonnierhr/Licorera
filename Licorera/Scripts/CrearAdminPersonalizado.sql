-- Script para crear usuario administrador con email jhonnierhr08@gmail.com
-- Ejecutar este script en SQL Server Management Studio

USE GestionNegocio;
GO

-- Verificar si el usuario ya existe
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'jhonnierhr08@gmail.com')
BEGIN
    DECLARE @AdminRoleId INT;
    
    -- Obtener el RolId del Admin
    SELECT @AdminRoleId = RolId FROM Roles WHERE Nombre = 'Admin';
    
    -- Insertar el nuevo usuario administrador
    INSERT INTO Usuarios (Nombre, Email, PasswordHash, RolId, CreatedAt)
    VALUES (
        'Jhonnier Administrator',
        'jhonnierhr08@gmail.com',
        'JAvlGEgOQzn8e10ATp0Q26kOc9COjchM/a7eP/4yhkQ=', -- Hash de "Admin123!"
        @AdminRoleId,
        GETDATE()
    );
    
    PRINT '? Usuario administrador jhonnierhr08@gmail.com creado exitosamente';
    PRINT '?? Email: jhonnierhr08@gmail.com';
    PRINT '?? Contraseña: Admin123!';
END
ELSE
BEGIN
    PRINT '?? El usuario jhonnierhr08@gmail.com ya existe';
END
GO