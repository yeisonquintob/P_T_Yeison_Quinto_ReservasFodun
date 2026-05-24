-- ============================================================
-- DB_ReservasFodun
-- Sistema de Reservas - Sedes Recreativas y Apartamentos FODUN
-- ============================================================
-- Script  : 01_CreateDatabase.sql
-- Autor   : Arquitecto BD Senior
-- Fecha   : 2026
-- Desc    : Creación de la base de datos principal
-- ============================================================

USE master;
GO

-- -------------------------------------------------------
-- Eliminar base de datos existente (solo en desarrollo)
-- -------------------------------------------------------
IF DB_ID('DB_ReservasFodun') IS NOT NULL
BEGIN
    ALTER DATABASE DB_ReservasFodun SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE DB_ReservasFodun;
    PRINT 'Base de datos DB_ReservasFodun eliminada correctamente.';
END
GO

-- -------------------------------------------------------
-- Crear base de datos
-- COLLATE: Modern_Spanish_CI_AI para soporte de acentos
-- -------------------------------------------------------
CREATE DATABASE DB_ReservasFodun
    COLLATE Modern_Spanish_CI_AI;
GO

PRINT 'Base de datos DB_ReservasFodun creada correctamente.';
GO

-- -------------------------------------------------------
-- Configurar opciones recomendadas
-- -------------------------------------------------------
USE DB_ReservasFodun;
GO

ALTER DATABASE DB_ReservasFodun
    SET RECOVERY SIMPLE;        -- En producción cambiar a FULL
GO

ALTER DATABASE DB_ReservasFodun
    SET READ_COMMITTED_SNAPSHOT ON;   -- Recomendado para EF Core
GO

PRINT 'Opciones de base de datos configuradas.';
GO

-- ============================================================
-- NOTA: Las tablas de ASP.NET Core Identity se generan
-- automáticamente mediante migrations de EF Core. Ejecutar
-- este script ANTES de correr: dotnet ef database update
-- La base de datos debe existir para que Identity la use.
-- ============================================================
