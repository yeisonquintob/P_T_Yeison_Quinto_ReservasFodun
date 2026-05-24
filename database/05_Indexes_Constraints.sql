-- ============================================================
-- DB_ReservasFodun
-- Sistema de Reservas - Sedes Recreativas y Apartamentos FODUN
-- ============================================================
-- Script  : 05_Indexes_Constraints.sql
-- Autor   : Arquitecto BD Senior
-- Fecha   : 2026
-- Desc    : Índices de rendimiento, FK con AspNetUsers
--           y restricciones adicionales.
-- ============================================================
-- IMPORTANTE:
--   Las FK con AspNetUsers (UserId) deben ejecutarse DESPUÉS
--   de correr: dotnet ef database update
--   que genera las tablas de Identity en la misma BD.
-- ============================================================

USE DB_ReservasFodun;
GO

-- ============================================================
-- SECCIÓN 1: ÍNDICES DE BÚSQUEDA Y RENDIMIENTO
-- ============================================================

-- -------------------------------------------------------
-- Tabla: Municipios
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Municipios_IdDepartamento')
    CREATE INDEX IX_Municipios_IdDepartamento
        ON dbo.Municipios (IdDepartamento)
        INCLUDE (Nombre);
GO

-- -------------------------------------------------------
-- Tabla: Sedes
-- Búsquedas frecuentes por tipo, municipio y estado activo
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Sedes_IdTipoSede')
    CREATE INDEX IX_Sedes_IdTipoSede
        ON dbo.Sedes (IdTipoSede, Activo)
        INCLUDE (Nombre, NombreCorto, IdMunicipio, CapacidadTotal);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Sedes_IdMunicipio')
    CREATE INDEX IX_Sedes_IdMunicipio
        ON dbo.Sedes (IdMunicipio, Activo)
        INCLUDE (Nombre, IdTipoSede);
GO

-- -------------------------------------------------------
-- Tabla: Alojamientos
-- Búsquedas frecuentes por sede y estado
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Alojamientos_IdSede')
    CREATE INDEX IX_Alojamientos_IdSede
        ON dbo.Alojamientos (IdSede, Activo)
        INCLUDE (Numero, Nombre, IdTipoAlojamiento, NumeroHabitaciones,
                 CapacidadMaxima, NumeroHabitacionesTarifa);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Alojamientos_IdTipoAlojamiento')
    CREATE INDEX IX_Alojamientos_IdTipoAlojamiento
        ON dbo.Alojamientos (IdTipoAlojamiento, Activo)
        INCLUDE (IdSede, NumeroHabitaciones, CapacidadMaxima);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Alojamientos_NumHabTarifa')
    CREATE INDEX IX_Alojamientos_NumHabTarifa
        ON dbo.Alojamientos (NumeroHabitacionesTarifa, IdSede, Activo)
        INCLUDE (CapacidadMaxima);
GO

-- -------------------------------------------------------
-- Tabla: Tarifas
-- Búsquedas por sede, alojamiento y temporada
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Tarifas_IdSede_IdTemporada')
    CREATE INDEX IX_Tarifas_IdSede_IdTemporada
        ON dbo.Tarifas (IdSede, IdTemporada, Activo)
        INCLUDE (TarifaBase, ValorPersonaAdicional, PersonasIncluidas,
                 NumeroHabitacionesTarifa, EsTarifaEspecial);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Tarifas_IdAlojamiento_IdTemporada')
    CREATE INDEX IX_Tarifas_IdAlojamiento_IdTemporada
        ON dbo.Tarifas (IdAlojamiento, IdTemporada, Activo)
        INCLUDE (TarifaBase, ValorPersonaAdicional, PersonasIncluidas, EsTarifaEspecial);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Tarifas_EsTarifaEspecial')
    CREATE INDEX IX_Tarifas_EsTarifaEspecial
        ON dbo.Tarifas (EsTarifaEspecial, Activo)
        INCLUDE (IdSede, IdAlojamiento, IdTemporada,
                 NumeroHabitacionesTarifa, TarifaBase, ValorPersonaAdicional);
GO

-- -------------------------------------------------------
-- Tabla: Festivos
-- Búsquedas por fecha (usada en sp_CalcularValorReserva)
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Festivos_Fecha')
    CREATE INDEX IX_Festivos_Fecha
        ON dbo.Festivos (Fecha, Activo);
GO

-- -------------------------------------------------------
-- Tabla: Reservas
-- Índices críticos: UserId, FechaLlegada, FechaSalida, IdSede, Estado
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Reservas_UserId')
    CREATE INDEX IX_Reservas_UserId
        ON dbo.Reservas (UserId, Activo)
        INCLUDE (CodigoReserva, IdSede, FechaLlegada, FechaSalida,
                 NumeroPersonas, ValorTotal, IdEstadoReserva);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Reservas_IdSede_Fechas')
    CREATE INDEX IX_Reservas_IdSede_Fechas
        ON dbo.Reservas (IdSede, FechaLlegada, FechaSalida, Activo)
        INCLUDE (IdEstadoReserva, UserId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Reservas_Fechas')
    CREATE INDEX IX_Reservas_Fechas
        ON dbo.Reservas (FechaLlegada, FechaSalida)
        INCLUDE (IdEstadoReserva, IdSede);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Reservas_IdEstadoReserva')
    CREATE INDEX IX_Reservas_IdEstadoReserva
        ON dbo.Reservas (IdEstadoReserva, Activo)
        INCLUDE (IdSede, FechaLlegada, FechaSalida, UserId);
GO

-- -------------------------------------------------------
-- Tabla: ReservaDetalles
-- Índice crítico: IdAlojamiento + cruce de fechas vía Reservas
-- Este es el índice más importante para sp_ConsultarDisponibilidad
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ReservaDetalles_IdAlojamiento')
    CREATE INDEX IX_ReservaDetalles_IdAlojamiento
        ON dbo.ReservaDetalles (IdAlojamiento)
        INCLUDE (IdReserva, ValorLinea);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ReservaDetalles_IdReserva')
    CREATE INDEX IX_ReservaDetalles_IdReserva
        ON dbo.ReservaDetalles (IdReserva)
        INCLUDE (IdAlojamiento);
GO

-- -------------------------------------------------------
-- Tabla: PerfilUsuario
-- Búsqueda por NroDocumento y UserId
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PerfilUsuario_NroDocumento')
    CREATE INDEX IX_PerfilUsuario_NroDocumento
        ON dbo.PerfilUsuario (NroDocumento)
        INCLUDE (UserId, Nombres, Apellidos);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PerfilUsuario_IdMunicipio')
    CREATE INDEX IX_PerfilUsuario_IdMunicipio
        ON dbo.PerfilUsuario (IdMunicipio);
GO

-- -------------------------------------------------------
-- Tabla: Pagos
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Pagos_IdReserva')
    CREATE INDEX IX_Pagos_IdReserva
        ON dbo.Pagos (IdReserva)
        INCLUDE (EstadoPago, ValorPagado, FechaPago);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Pagos_EstadoPago')
    CREATE INDEX IX_Pagos_EstadoPago
        ON dbo.Pagos (EstadoPago)
        INCLUDE (IdReserva, ValorPagado);
GO

-- -------------------------------------------------------
-- Tabla: AlojamientoCaracteristicas
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AloCarac_IdCaracteristica')
    CREATE INDEX IX_AloCarac_IdCaracteristica
        ON dbo.AlojamientoCaracteristicas (IdCaracteristica)
        INCLUDE (IdAlojamiento, Cantidad);
GO

PRINT 'Índices creados correctamente.';
GO

-- ============================================================
-- SECCIÓN 2: FK CON ASPNETUSERS (EJECUTAR POST-MIGRATION)
-- ============================================================
-- Ejecutar SOLO después de: dotnet ef database update
-- que habrá creado la tabla dbo.AspNetUsers en esta BD.
--
-- Para ejecutar condicionalmente:
-- ============================================================

IF OBJECT_ID('dbo.AspNetUsers', 'U') IS NOT NULL
BEGIN
    -- FK en PerfilUsuario.UserId → AspNetUsers.Id
    IF NOT EXISTS (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = 'FK_PerfilUsuario_AspNetUsers'
    )
    BEGIN
        ALTER TABLE dbo.PerfilUsuario
            ADD CONSTRAINT FK_PerfilUsuario_AspNetUsers
                FOREIGN KEY (UserId)
                REFERENCES dbo.AspNetUsers(Id)
                ON DELETE CASCADE;
        PRINT 'FK_PerfilUsuario_AspNetUsers creada.';
    END

    -- FK en Reservas.UserId → AspNetUsers.Id
    IF NOT EXISTS (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = 'FK_Reservas_AspNetUsers'
    )
    BEGIN
        ALTER TABLE dbo.Reservas
            ADD CONSTRAINT FK_Reservas_AspNetUsers
                FOREIGN KEY (UserId)
                REFERENCES dbo.AspNetUsers(Id);
        PRINT 'FK_Reservas_AspNetUsers creada.';
    END

    -- Índice en AspNetUsers por campo de búsqueda personalizado (NroDocumento)
    -- Este campo existe en AspNetUsers si se extiende ApplicationUser
    IF EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID('dbo.AspNetUsers')
        AND   name = 'NroDocumento'
    )
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AspNetUsers_NroDocumento')
            CREATE INDEX IX_AspNetUsers_NroDocumento
                ON dbo.AspNetUsers (NroDocumento);
        PRINT 'IX_AspNetUsers_NroDocumento creado.';
    END
END
ELSE
BEGIN
    PRINT '--- AVISO: AspNetUsers no existe aún.';
    PRINT '--- Ejecutar primero: dotnet ef database update';
    PRINT '--- Luego volver a correr la Sección 2 de este script.';
END
GO

-- ============================================================
-- SECCIÓN 3: RESTRICCIONES CHECK ADICIONALES
-- ============================================================

-- Validar que FechaLlegada no sea en el pasado al insertar
-- (solo aplica a inserciones nuevas, no a históricos)
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_Reservas_FechaLlegadaMinima'
)
BEGIN
    -- NOTA: Esta restricción se omite para permitir datos históricos de prueba.
    -- En producción, la validación de fecha mínima se hace en el SP y en la app.
    PRINT 'CK_Reservas_FechaLlegadaMinima: omitida (validada en SP y capa de aplicación).';
END
GO

-- ============================================================
-- SECCIÓN 4: ESTADÍSTICAS Y MANTENIMIENTO
-- ============================================================

-- Actualizar estadísticas en las tablas principales
-- (ejecutar periódicamente en producción)
UPDATE STATISTICS dbo.Alojamientos;
UPDATE STATISTICS dbo.Reservas;
UPDATE STATISTICS dbo.ReservaDetalles;
UPDATE STATISTICS dbo.Tarifas;
GO

PRINT '=== 05_Indexes_Constraints.sql ejecutado correctamente. ===';
GO
