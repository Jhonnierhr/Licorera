-- Script para agregar campo EliminadoPorAdmin a la tabla Pedidos
-- Ejecutar este script en SQL Server Management Studio

USE GestionNegocio;
GO

-- Agregar columna EliminadoPorAdmin (por defecto FALSE para pedidos existentes)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Pedidos' AND COLUMN_NAME = 'EliminadoPorAdmin')
BEGIN
    ALTER TABLE Pedidos 
    ADD EliminadoPorAdmin BIT NOT NULL DEFAULT 0;
    
    PRINT 'Campo EliminadoPorAdmin agregado exitosamente a la tabla Pedidos';
END
ELSE
BEGIN
    PRINT 'El campo EliminadoPorAdmin ya existe en la tabla Pedidos';
END
GO