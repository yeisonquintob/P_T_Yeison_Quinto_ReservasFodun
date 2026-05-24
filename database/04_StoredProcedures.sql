-- ============================================================
-- DB_ReservasFodun
-- Sistema de Reservas - Sedes Recreativas y Apartamentos FODUN
-- ============================================================
-- Script  : 04_StoredProcedures.sql
-- Autor   : Arquitecto BD Senior
-- Fecha   : 2026
-- Desc    : Procedimientos almacenados del sistema
-- ============================================================
-- CONTENIDO:
--  1. sp_ConsultarAlojamientosDisponiblesPorFecha        [OBLIGATORIO]
--  2. sp_ConsultarAlojamientosDisponiblesPorFechaPersonas [OBLIGATORIO]
--  3. sp_ConsultarTarifas                                [OBLIGATORIO]
--  4. sp_CalcularValorReserva                            [OBLIGATORIO]
--  5. sp_CrearReserva                                    [ADICIONAL]
--  6. sp_ConsultarReservasUsuario                        [ADICIONAL]
--  7. sp_CancelarReserva                                 [ADICIONAL]
--  8. sp_ConsultarCalendarioDisponibilidad               [ADICIONAL]
-- ============================================================

USE DB_ReservasFodun;
GO

-- ============================================================
-- 1. sp_ConsultarAlojamientosDisponiblesPorFecha
--    Encuentra alojamientos disponibles en un rango de fechas.
--    Excluye reservas canceladas (no bloquean disponibilidad).
-- ============================================================
IF OBJECT_ID('dbo.sp_ConsultarAlojamientosDisponiblesPorFecha', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ConsultarAlojamientosDisponiblesPorFecha;
GO

CREATE PROCEDURE dbo.sp_ConsultarAlojamientosDisponiblesPorFecha
    @p_FechaLlegada     DATE,
    @p_FechaSalida      DATE,
    @p_IdSede           INT         = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- -------------------------------------------------------
    -- Validaciones de entrada
    -- -------------------------------------------------------
    IF @p_FechaLlegada IS NULL OR @p_FechaSalida IS NULL
    BEGIN
        RAISERROR('Las fechas de llegada y salida son obligatorias.', 16, 1);
        RETURN;
    END

    IF @p_FechaSalida <= @p_FechaLlegada
    BEGIN
        RAISERROR('La fecha de salida debe ser posterior a la fecha de llegada.', 16, 1);
        RETURN;
    END

    IF @p_FechaLlegada < CAST(GETDATE() AS DATE)
    BEGIN
        RAISERROR('La fecha de llegada no puede ser anterior a hoy.', 16, 1);
        RETURN;
    END

    -- -------------------------------------------------------
    -- Consulta principal
    -- Lógica de cruce de fechas:
    --   Hay conflicto cuando:
    --   ReservaExistente.FechaLlegada  < @p_FechaSalida
    --   AND ReservaExistente.FechaSalida > @p_FechaLlegada
    --
    -- Se excluyen reservas canceladas (BloqueoDisponibilidad=0)
    -- -------------------------------------------------------
    SELECT
        s.IdSede,
        s.Nombre                        AS NombreSede,
        s.NombreCorto,
        ts.Nombre                       AS TipoSede,
        m.Nombre                        AS Ciudad,
        a.IdAlojamiento,
        a.Numero                        AS NumeroAlojamiento,
        a.Nombre                        AS NombreAlojamiento,
        ta.Nombre                       AS TipoAlojamiento,
        a.NumeroHabitaciones,
        a.CapacidadMaxima               AS Capacidad,
        a.NumeroHabitacionesTarifa,
        a.Descripcion,
        -- Disponible: 1 si no existe reserva activa que cruce las fechas
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM   dbo.ReservaDetalles rd
                INNER JOIN dbo.Reservas r  ON rd.IdReserva = r.IdReserva
                INNER JOIN dbo.EstadosReserva er ON r.IdEstadoReserva = er.IdEstadoReserva
                WHERE  rd.IdAlojamiento = a.IdAlojamiento
                AND    er.BloqueoDisponibilidad = 1      -- solo estados que bloquean
                AND    r.FechaLlegada  < @p_FechaSalida
                AND    r.FechaSalida   > @p_FechaLlegada
            )
            THEN 0
            ELSE 1
        END                             AS Disponible,
        DATEDIFF(DAY, @p_FechaLlegada, @p_FechaSalida) AS NumeroNoches
    FROM dbo.Alojamientos a
    INNER JOIN dbo.Sedes s              ON a.IdSede = s.IdSede
    INNER JOIN dbo.TiposSede ts         ON s.IdTipoSede = ts.IdTipoSede
    INNER JOIN dbo.Municipios m         ON s.IdMunicipio = m.IdMunicipio
    INNER JOIN dbo.TiposAlojamiento ta  ON a.IdTipoAlojamiento = ta.IdTipoAlojamiento
    WHERE a.Activo = 1
    AND   s.Activo = 1
    AND   (@p_IdSede IS NULL OR a.IdSede = @p_IdSede)
    ORDER BY s.Nombre, a.Numero;
END;
GO

PRINT 'SP sp_ConsultarAlojamientosDisponiblesPorFecha creado.';

-- ============================================================
-- 2. sp_ConsultarAlojamientosDisponiblesPorFechaPersonas
--    Como el anterior pero filtra por capacidad mínima.
-- ============================================================
IF OBJECT_ID('dbo.sp_ConsultarAlojamientosDisponiblesPorFechaPersonas', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ConsultarAlojamientosDisponiblesPorFechaPersonas;
GO

CREATE PROCEDURE dbo.sp_ConsultarAlojamientosDisponiblesPorFechaPersonas
    @p_FechaLlegada     DATE,
    @p_FechaSalida      DATE,
    @p_NumeroPersonas   INT,
    @p_IdSede           INT         = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- -------------------------------------------------------
    -- Validaciones
    -- -------------------------------------------------------
    IF @p_FechaLlegada IS NULL OR @p_FechaSalida IS NULL
    BEGIN
        RAISERROR('Las fechas de llegada y salida son obligatorias.', 16, 1);
        RETURN;
    END

    IF @p_FechaSalida <= @p_FechaLlegada
    BEGIN
        RAISERROR('La fecha de salida debe ser posterior a la fecha de llegada.', 16, 1);
        RETURN;
    END

    IF @p_NumeroPersonas IS NULL OR @p_NumeroPersonas <= 0
    BEGIN
        RAISERROR('El número de personas debe ser mayor a cero.', 16, 1);
        RETURN;
    END

    IF @p_FechaLlegada < CAST(GETDATE() AS DATE)
    BEGIN
        RAISERROR('La fecha de llegada no puede ser anterior a hoy.', 16, 1);
        RETURN;
    END

    -- -------------------------------------------------------
    -- Variables internas
    -- -------------------------------------------------------
    DECLARE @v_NumeroNoches INT = DATEDIFF(DAY, @p_FechaLlegada, @p_FechaSalida);

    -- -------------------------------------------------------
    -- Resultado: alojamientos disponibles con capacidad >= personas solicitadas
    -- -------------------------------------------------------
    SELECT
        s.IdSede,
        s.Nombre                        AS NombreSede,
        s.NombreCorto,
        ts.Nombre                       AS TipoSede,
        m.Nombre                        AS Ciudad,
        a.IdAlojamiento,
        a.Numero                        AS NumeroAlojamiento,
        a.Nombre                        AS NombreAlojamiento,
        ta.Nombre                       AS TipoAlojamiento,
        a.NumeroHabitaciones,
        a.CapacidadMaxima               AS Capacidad,
        a.NumeroHabitacionesTarifa,
        a.Descripcion,
        @v_NumeroNoches                 AS NumeroNoches,
        @p_NumeroPersonas               AS PersonasSolicitadas,
        -- PersonasSobrante: cuántas personas supera la capacidad del alojamiento
        CASE
            WHEN @p_NumeroPersonas > a.CapacidadMaxima
            THEN @p_NumeroPersonas - a.CapacidadMaxima
            ELSE 0
        END                             AS PersonasExcedentes
    FROM dbo.Alojamientos a
    INNER JOIN dbo.Sedes s              ON a.IdSede = s.IdSede
    INNER JOIN dbo.TiposSede ts         ON s.IdTipoSede = ts.IdTipoSede
    INNER JOIN dbo.Municipios m         ON s.IdMunicipio = m.IdMunicipio
    INNER JOIN dbo.TiposAlojamiento ta  ON a.IdTipoAlojamiento = ta.IdTipoAlojamiento
    WHERE a.Activo = 1
    AND   s.Activo = 1
    AND   (@p_IdSede IS NULL OR a.IdSede = @p_IdSede)
    -- Capacidad suficiente para al menos una persona
    -- (se puede reservar más de un alojamiento para cubrir el grupo)
    AND   a.CapacidadMaxima >= 1
    -- No existe reserva activa que cruce las fechas
    AND NOT EXISTS (
        SELECT 1
        FROM   dbo.ReservaDetalles rd
        INNER JOIN dbo.Reservas r  ON rd.IdReserva = r.IdReserva
        INNER JOIN dbo.EstadosReserva er ON r.IdEstadoReserva = er.IdEstadoReserva
        WHERE  rd.IdAlojamiento = a.IdAlojamiento
        AND    er.BloqueoDisponibilidad = 1
        AND    r.FechaLlegada  < @p_FechaSalida
        AND    r.FechaSalida   > @p_FechaLlegada
    )
    ORDER BY
        -- Priorizar alojamientos cuya capacidad se ajuste mejor al grupo
        ABS(a.CapacidadMaxima - @p_NumeroPersonas),
        s.Nombre,
        a.Numero;
END;
GO

PRINT 'SP sp_ConsultarAlojamientosDisponiblesPorFechaPersonas creado.';

-- ============================================================
-- 3. sp_ConsultarTarifas
--    Consulta tarifas según sede, alojamiento, temporada y personas.
--    Devuelve tarifa ordinaria y especial cuando aplica.
-- ============================================================
IF OBJECT_ID('dbo.sp_ConsultarTarifas', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ConsultarTarifas;
GO

CREATE PROCEDURE dbo.sp_ConsultarTarifas
    @p_IdSede           INT,
    @p_IdAlojamiento    INT         = NULL,
    @p_IdTemporada      INT         = NULL,    -- NULL = devuelve todas las temporadas del alojamiento
    @p_NumeroPersonas   INT         = 1
AS
BEGIN
    SET NOCOUNT ON;

    -- -------------------------------------------------------
    -- Validaciones
    -- -------------------------------------------------------
    IF @p_IdSede IS NULL
    BEGIN
        RAISERROR('El parámetro IdSede es obligatorio.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Sedes WHERE IdSede = @p_IdSede AND Activo = 1)
    BEGIN
        RAISERROR('La sede especificada no existe o está inactiva.', 16, 1);
        RETURN;
    END

    IF @p_NumeroPersonas IS NULL OR @p_NumeroPersonas <= 0
        SET @p_NumeroPersonas = 1;

    -- -------------------------------------------------------
    -- Variables internas
    -- -------------------------------------------------------
    DECLARE @v_NumHabTarifa INT = NULL;

    -- Si se especifica alojamiento, obtener su NumHabTarifa
    IF @p_IdAlojamiento IS NOT NULL
        SELECT @v_NumHabTarifa = NumeroHabitacionesTarifa
        FROM   dbo.Alojamientos
        WHERE  IdAlojamiento = @p_IdAlojamiento AND Activo = 1;

    -- -------------------------------------------------------
    -- Consulta de tarifas con prioridad:
    --   1. Tarifa por alojamiento específico
    --   2. Tarifa por sede específica
    --   3. Tarifa genérica (IdSede NULL, para sedes recreativas)
    -- -------------------------------------------------------
    ;WITH TarifasRanked AS (
        SELECT
            t.IdTarifa,
            t.IdSede,
            t.IdAlojamiento,
            t.IdTemporada,
            t.NumeroHabitacionesTarifa,
            t.PersonasIncluidas,
            t.TarifaBase,
            t.ValorPersonaAdicional,
            t.EsTarifaEspecial,
            t.Descripcion,
            -- Prioridad: específico de alojamiento > específico de sede > genérico
            CASE
                WHEN t.IdAlojamiento IS NOT NULL THEN 1
                WHEN t.IdSede        IS NOT NULL THEN 2
                ELSE 3
            END AS Prioridad
        FROM dbo.Tarifas t
        WHERE t.Activo = 1
        -- Filtro por temporada (si se especifica)
        AND (@p_IdTemporada IS NULL OR t.IdTemporada = @p_IdTemporada)
        -- Filtro por alojamiento específico O por sede O genérico
        AND (
            (t.IdAlojamiento = @p_IdAlojamiento AND @p_IdAlojamiento IS NOT NULL)
            OR
            (t.IdSede = @p_IdSede AND t.IdAlojamiento IS NULL)
            OR
            (t.IdSede IS NULL AND t.IdAlojamiento IS NULL
             AND (@v_NumHabTarifa IS NULL OR t.NumeroHabitacionesTarifa = @v_NumHabTarifa)
            )
        )
    ),
    TarifasConRango AS (
        SELECT *,
            ROW_NUMBER() OVER (
                PARTITION BY IdTemporada, EsTarifaEspecial
                ORDER BY Prioridad
            ) AS RowNum
        FROM TarifasRanked
    )
    SELECT
        s.IdSede,
        s.Nombre                            AS NombreSede,
        s.NombreCorto,
        ISNULL(a.IdAlojamiento, 0)          AS IdAlojamiento,
        ISNULL(a.Nombre, 'Todos')           AS NombreAlojamiento,
        a.CapacidadMaxima                   AS Capacidad,
        temp.Nombre                         AS Temporada,
        temp.EsTemporadaAlta,
        temp.EsTarifaEspecial               AS EsTemporadaEspecial,
        tc.NumeroHabitacionesTarifa,
        tc.PersonasIncluidas,
        @p_NumeroPersonas                   AS NumeroPersonas,
        CASE
            WHEN @p_NumeroPersonas > tc.PersonasIncluidas
            THEN @p_NumeroPersonas - tc.PersonasIncluidas
            ELSE 0
        END                                 AS PersonasAdicionales,
        tc.TarifaBase,
        tc.ValorPersonaAdicional,
        -- ValorEstimado por noche (sin multiplicar por número de noches)
        tc.TarifaBase
        + (
            CASE
                WHEN @p_NumeroPersonas > tc.PersonasIncluidas
                THEN (@p_NumeroPersonas - tc.PersonasIncluidas) * tc.ValorPersonaAdicional
                ELSE 0
            END
          )                                 AS ValorEstimadoPorNoche
    FROM TarifasConRango tc
    INNER JOIN dbo.Temporadas temp          ON tc.IdTemporada = temp.IdTemporada
    INNER JOIN dbo.Sedes s                  ON s.IdSede = @p_IdSede
    LEFT  JOIN dbo.Alojamientos a           ON tc.IdAlojamiento = a.IdAlojamiento
    WHERE tc.RowNum = 1
    ORDER BY temp.EsTarifaEspecial, temp.EsTemporadaAlta;
END;
GO

PRINT 'SP sp_ConsultarTarifas creado.';

-- ============================================================
-- 4. sp_CalcularValorReserva
--    Calcula el valor total a cancelar considerando:
--    - Noches ordinarias vs. especiales (Lun-Jue promo)
--    - Personas adicionales
--    - Servicio de lavandería
--    - Festivos (excluyen tarifa especial)
-- ============================================================
IF OBJECT_ID('dbo.sp_CalcularValorReserva', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CalcularValorReserva;
GO

CREATE PROCEDURE dbo.sp_CalcularValorReserva
    @p_IdSede               INT,
    @p_IdAlojamiento        INT         = NULL,
    @p_IdTipoAlojamiento    INT         = NULL,
    @p_NumeroHabitaciones   INT         = 1,
    @p_NumeroPersonas       INT,
    @p_FechaLlegada         DATE,
    @p_FechaSalida          DATE,
    @p_IncluyeLavanderia    BIT         = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- -------------------------------------------------------
    -- Validaciones
    -- -------------------------------------------------------
    IF @p_FechaLlegada IS NULL OR @p_FechaSalida IS NULL
    BEGIN
        RAISERROR('Las fechas de llegada y salida son obligatorias.', 16, 1);
        RETURN;
    END

    IF @p_FechaSalida <= @p_FechaLlegada
    BEGIN
        RAISERROR('La fecha de salida debe ser posterior a la fecha de llegada.', 16, 1);
        RETURN;
    END

    IF @p_NumeroPersonas IS NULL OR @p_NumeroPersonas <= 0
    BEGIN
        RAISERROR('El número de personas debe ser mayor a cero.', 16, 1);
        RETURN;
    END

    IF @p_NumeroHabitaciones IS NULL OR @p_NumeroHabitaciones <= 0
        SET @p_NumeroHabitaciones = 1;

    -- -------------------------------------------------------
    -- Variables internas
    -- -------------------------------------------------------
    DECLARE
        @v_NumeroNoches         INT,
        @v_DiasEspeciales       INT             = 0,
        @v_DiasOrdinarios       INT             = 0,
        @v_FechaIter            DATE,
        @v_DowFecha             INT,

        -- Tarifa ordinaria
        @v_TarifaOrd            DECIMAL(18,2)   = 0,
        @v_PersonasInclOrd      INT             = 0,
        @v_ValorPersAdicOrd     DECIMAL(18,2)   = 0,

        -- Tarifa especial (Lun-Jue promo)
        @v_TarifaEsp            DECIMAL(18,2)   = 0,
        @v_PersonasInclEsp      INT             = 0,
        @v_ValorPersAdicEsp     DECIMAL(18,2)   = 0,
        @v_ExisteTarifaEsp      BIT             = 0,

        -- NumHabTarifa del alojamiento
        @v_NumHabTarifa         INT             = NULL,

        -- Cálculos
        @v_PersonasAdicionales  INT             = 0,
        @v_ValorOrdinario       DECIMAL(18,2)   = 0,
        @v_ValorEspecial        DECIMAL(18,2)   = 0,
        @v_ValorPersonasOrd     DECIMAL(18,2)   = 0,
        @v_ValorPersonasEsp     DECIMAL(18,2)   = 0,
        @v_ValorLavanderia      DECIMAL(18,2)   = 0,
        @v_ValorSubtotal        DECIMAL(18,2)   = 0,
        @v_ValorTotal           DECIMAL(18,2)   = 0,

        -- Info sede/alojamiento
        @v_NombreSede           NVARCHAR(200),
        @v_NombreAlojamiento    NVARCHAR(200),
        @v_TipoSede             NVARCHAR(100),
        @v_EsTemporadaAlta      BIT             = 0;

    -- -------------------------------------------------------
    -- Número de noches
    -- -------------------------------------------------------
    SET @v_NumeroNoches = DATEDIFF(DAY, @p_FechaLlegada, @p_FechaSalida);

    -- -------------------------------------------------------
    -- Obtener nombres informativos
    -- -------------------------------------------------------
    SELECT
        @v_NombreSede  = s.Nombre,
        @v_TipoSede    = ts.Nombre
    FROM dbo.Sedes s
    INNER JOIN dbo.TiposSede ts ON s.IdTipoSede = ts.IdTipoSede
    WHERE s.IdSede = @p_IdSede;

    IF @p_IdAlojamiento IS NOT NULL
    BEGIN
        SELECT
            @v_NombreAlojamiento = a.Nombre,
            @v_NumHabTarifa      = a.NumeroHabitacionesTarifa
        FROM dbo.Alojamientos a
        WHERE a.IdAlojamiento = @p_IdAlojamiento;
    END
    ELSE
    BEGIN
        -- Para sedes recreativas sin alojamiento específico,
        -- usar NumHabitaciones para determinar la categoría tarifaria
        SET @v_NumHabTarifa      = @p_NumeroHabitaciones;
        SET @v_NombreAlojamiento = CAST(@p_NumeroHabitaciones AS NVARCHAR) + ' habitación(es)';
    END

    -- -------------------------------------------------------
    -- Obtener tarifa ordinaria (o de temporada del alojamiento)
    -- Prioridad: específico de alojamiento > sede > genérico
    -- -------------------------------------------------------
    SELECT TOP 1
        @v_TarifaOrd        = t.TarifaBase,
        @v_PersonasInclOrd  = t.PersonasIncluidas,
        @v_ValorPersAdicOrd = t.ValorPersonaAdicional
    FROM dbo.Tarifas t
    INNER JOIN dbo.Temporadas temp ON t.IdTemporada = temp.IdTemporada
    WHERE t.Activo = 1
    AND   t.EsTarifaEspecial = 0
    -- Para Santa Marta en alta temporada
    AND   temp.EsTemporadaAlta = @v_EsTemporadaAlta
    AND (
            (t.IdAlojamiento = @p_IdAlojamiento AND @p_IdAlojamiento IS NOT NULL)
        OR  (t.IdSede = @p_IdSede AND t.IdAlojamiento IS NULL AND @p_IdAlojamiento IS NULL)
        OR  (t.IdSede IS NULL AND t.IdAlojamiento IS NULL
             AND (t.NumeroHabitacionesTarifa = @v_NumHabTarifa OR t.NumeroHabitacionesTarifa IS NULL))
    )
    ORDER BY
        CASE WHEN t.IdAlojamiento IS NOT NULL THEN 1
             WHEN t.IdSede IS NOT NULL         THEN 2
             ELSE 3 END;

    -- Si no se encontró tarifa específica, buscar por alta temporada para Santa Marta
    IF @v_TarifaOrd = 0 AND @p_IdAlojamiento IS NOT NULL
    BEGIN
        SELECT TOP 1
            @v_TarifaOrd        = t.TarifaBase,
            @v_PersonasInclOrd  = t.PersonasIncluidas,
            @v_ValorPersAdicOrd = t.ValorPersonaAdicional
        FROM dbo.Tarifas t
        WHERE t.Activo = 1
        AND   t.EsTarifaEspecial = 0
        AND   t.IdAlojamiento = @p_IdAlojamiento
        ORDER BY t.IdTemporada;
    END

    -- -------------------------------------------------------
    -- Obtener tarifa especial Lun-Jue (si existe para esta sede)
    -- Solo para sedes recreativas con NumHabTarifa definido
    -- -------------------------------------------------------
    IF @v_NumHabTarifa IS NOT NULL
    BEGIN
        SELECT TOP 1
            @v_TarifaEsp        = t.TarifaBase,
            @v_PersonasInclEsp  = t.PersonasIncluidas,
            @v_ValorPersAdicEsp = t.ValorPersonaAdicional,
            @v_ExisteTarifaEsp  = 1
        FROM dbo.Tarifas t
        INNER JOIN dbo.Temporadas temp ON t.IdTemporada = temp.IdTemporada
        WHERE t.Activo = 1
        AND   t.EsTarifaEspecial = 1
        AND   temp.EsTarifaEspecial = 1
        AND (
                (t.IdSede = @p_IdSede AND t.IdAlojamiento IS NULL)
            OR  (t.IdSede IS NULL AND t.IdAlojamiento IS NULL
                 AND t.NumeroHabitacionesTarifa = @v_NumHabTarifa)
        )
        ORDER BY
            CASE WHEN t.IdSede IS NOT NULL THEN 1 ELSE 2 END;
    END

    -- -------------------------------------------------------
    -- Contar días ordinarios y especiales en el rango
    -- Día especial = Lun-Jue + NO es festivo
    -- Con SET DATEFIRST 7: 1=Dom,2=Lun,3=Mar,4=Mié,5=Jue,6=Vie,7=Sáb
    -- -------------------------------------------------------
    IF @v_ExisteTarifaEsp = 1
    BEGIN
        SET @v_FechaIter = @p_FechaLlegada;
        WHILE @v_FechaIter < @p_FechaSalida
        BEGIN
            SET @v_DowFecha = DATEPART(dw, @v_FechaIter);
            -- Lun-Jue (dw 2-5) y no es festivo
            IF @v_DowFecha IN (2, 3, 4, 5)
               AND NOT EXISTS (
                    SELECT 1 FROM dbo.Festivos
                    WHERE Fecha = @v_FechaIter AND Activo = 1
               )
            BEGIN
                SET @v_DiasEspeciales = @v_DiasEspeciales + 1;
            END
            ELSE
            BEGIN
                SET @v_DiasOrdinarios = @v_DiasOrdinarios + 1;
            END
            SET @v_FechaIter = DATEADD(DAY, 1, @v_FechaIter);
        END;
    END
    ELSE
    BEGIN
        -- Sin tarifa especial: todos los días son ordinarios
        SET @v_DiasOrdinarios = @v_NumeroNoches;
        SET @v_DiasEspeciales = 0;
    END;

    -- -------------------------------------------------------
    -- Calcular personas adicionales (basado en tarifa ordinaria)
    -- -------------------------------------------------------
    SET @v_PersonasAdicionales = CASE
        WHEN @p_NumeroPersonas > @v_PersonasInclOrd
        THEN @p_NumeroPersonas - @v_PersonasInclOrd
        ELSE 0
    END;

    -- -------------------------------------------------------
    -- Calcular valores por tipo de día
    -- -------------------------------------------------------

    -- Valor alojamiento días ordinarios
    SET @v_ValorOrdinario = @v_DiasOrdinarios * @v_TarifaOrd * @p_NumeroHabitaciones;

    -- Cargo personas adicionales en días ordinarios
    SET @v_ValorPersonasOrd = @v_DiasOrdinarios * @v_PersonasAdicionales * @v_ValorPersAdicOrd;

    -- Valor alojamiento días especiales (promo Lun-Jue)
    IF @v_ExisteTarifaEsp = 1
    BEGIN
        SET @v_ValorEspecial = @v_DiasEspeciales * @v_TarifaEsp * @p_NumeroHabitaciones;
        DECLARE @v_PersAdicEsp INT = CASE
            WHEN @p_NumeroPersonas > @v_PersonasInclEsp
            THEN @p_NumeroPersonas - @v_PersonasInclEsp
            ELSE 0
        END;
        SET @v_ValorPersonasEsp = @v_DiasEspeciales * @v_PersAdicEsp * @v_ValorPersAdicEsp;
    END
    ELSE
    BEGIN
        SET @v_ValorEspecial   = 0;
        SET @v_ValorPersonasEsp = 0;
    END;

    -- -------------------------------------------------------
    -- Lavandería (precio fijo, no por noche)
    -- -------------------------------------------------------
    IF @p_IncluyeLavanderia = 1
    BEGIN
        SELECT @v_ValorLavanderia = Valor
        FROM   dbo.ServiciosAdicionales
        WHERE  Nombre = 'Servicio de Lavandería' AND Activo = 1;

        SET @v_ValorLavanderia = ISNULL(@v_ValorLavanderia, 0);
    END;

    -- -------------------------------------------------------
    -- Totales
    -- -------------------------------------------------------
    SET @v_ValorSubtotal = @v_ValorOrdinario
                         + @v_ValorPersonasOrd
                         + @v_ValorEspecial
                         + @v_ValorPersonasEsp;

    SET @v_ValorTotal    = @v_ValorSubtotal + @v_ValorLavanderia;

    -- -------------------------------------------------------
    -- Resultado: resumen del cálculo
    -- -------------------------------------------------------
    SELECT
        @p_IdSede                   AS IdSede,
        @v_NombreSede               AS NombreSede,
        @v_TipoSede                 AS TipoSede,
        ISNULL(@p_IdAlojamiento, 0) AS IdAlojamiento,
        @v_NombreAlojamiento        AS NombreAlojamiento,
        @p_FechaLlegada             AS FechaLlegada,
        @p_FechaSalida              AS FechaSalida,
        @v_NumeroNoches             AS NumeroNoches,
        @p_NumeroPersonas           AS NumeroPersonas,
        @p_NumeroHabitaciones       AS NumeroHabitaciones,
        @v_NumHabTarifa             AS NumeroHabitacionesTarifa,
        -- Desglose de días
        @v_DiasOrdinarios           AS DiasOrdinarios,
        @v_DiasEspeciales           AS DiasEspeciales,
        -- Tarifas aplicadas
        @v_TarifaOrd                AS TarifaOrdinariaPorNoche,
        @v_PersonasInclOrd          AS PersonasIncluidasOrdinario,
        @v_ValorPersAdicOrd         AS ValorPersonaAdicionalOrdinario,
        CASE WHEN @v_ExisteTarifaEsp = 1 THEN @v_TarifaEsp   ELSE NULL END AS TarifaEspecialPorNoche,
        CASE WHEN @v_ExisteTarifaEsp = 1 THEN @v_PersonasInclEsp ELSE NULL END AS PersonasIncluidasEspecial,
        CASE WHEN @v_ExisteTarifaEsp = 1 THEN @v_ValorPersAdicEsp ELSE NULL END AS ValorPersonaAdicionalEspecial,
        -- Personas adicionales
        @v_PersonasAdicionales      AS PersonasAdicionales,
        -- Desglose de valor
        @v_ValorOrdinario           AS ValorTarifaOrdinaria,
        @v_ValorPersonasOrd         AS ValorPersonasAdicionalesOrdinario,
        @v_ValorEspecial            AS ValorTarifaEspecial,
        @v_ValorPersonasEsp         AS ValorPersonasAdicionalesEspecial,
        @v_ValorLavanderia          AS ValorLavanderia,
        @p_IncluyeLavanderia        AS IncluyeLavanderia,
        @v_ValorSubtotal            AS ValorSubtotal,
        @v_ValorTotal               AS ValorTotal;
END;
GO

PRINT 'SP sp_CalcularValorReserva creado.';

-- ============================================================
-- 5. sp_CrearReserva
--    Crea una nueva reserva validando disponibilidad.
--    Recibe lista de IdAlojamientos como XML.
-- ============================================================
IF OBJECT_ID('dbo.sp_CrearReserva', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CrearReserva;
GO

CREATE PROCEDURE dbo.sp_CrearReserva
    @p_UserId               NVARCHAR(450),
    @p_IdSede               INT,
    @p_FechaLlegada         DATE,
    @p_FechaSalida          DATE,
    @p_NumeroPersonas       INT,
    @p_NumeroHabitaciones   INT         = 1,
    @p_IncluyeLavanderia    BIT         = 0,
    @p_Observaciones        NVARCHAR(1000) = NULL,
    -- XML con alojamientos: <alojamientos><id>1</id><id>2</id></alojamientos>
    @p_AlojamientosXml      XML
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- -------------------------------------------------------
    -- Validaciones
    -- -------------------------------------------------------
    IF @p_UserId IS NULL OR LEN(TRIM(@p_UserId)) = 0
    BEGIN
        RAISERROR('El usuario es obligatorio.', 16, 1); RETURN;
    END

    IF @p_FechaSalida <= @p_FechaLlegada
    BEGIN
        RAISERROR('La fecha de salida debe ser posterior a la fecha de llegada.', 16, 1); RETURN;
    END

    IF @p_NumeroPersonas <= 0
    BEGIN
        RAISERROR('El número de personas debe ser mayor a cero.', 16, 1); RETURN;
    END

    -- -------------------------------------------------------
    -- Parsear lista de alojamientos desde XML
    -- -------------------------------------------------------
    DECLARE @v_Alojamientos TABLE (IdAlojamiento INT);

    INSERT INTO @v_Alojamientos
    SELECT x.value('.', 'INT')
    FROM   @p_AlojamientosXml.nodes('/alojamientos/id') AS t(x);

    IF NOT EXISTS (SELECT 1 FROM @v_Alojamientos)
    BEGIN
        RAISERROR('Debe especificar al menos un alojamiento.', 16, 1); RETURN;
    END

    -- -------------------------------------------------------
    -- Validar disponibilidad de TODOS los alojamientos
    -- -------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM   @v_Alojamientos va
        WHERE  EXISTS (
            SELECT 1
            FROM   dbo.ReservaDetalles rd
            INNER JOIN dbo.Reservas r ON rd.IdReserva = r.IdReserva
            INNER JOIN dbo.EstadosReserva er ON r.IdEstadoReserva = er.IdEstadoReserva
            WHERE  rd.IdAlojamiento = va.IdAlojamiento
            AND    er.BloqueoDisponibilidad = 1
            AND    r.FechaLlegada < @p_FechaSalida
            AND    r.FechaSalida  > @p_FechaLlegada
        )
    )
    BEGIN
        RAISERROR('Uno o más alojamientos no están disponibles en las fechas seleccionadas.', 16, 1);
        RETURN;
    END

    -- -------------------------------------------------------
    -- Calcular valor de la reserva
    -- -------------------------------------------------------
    DECLARE
        @v_NumeroNoches         INT,
        @v_DiasOrdinarios       INT     = 0,
        @v_DiasEspeciales       INT     = 0,
        @v_TarifaOrd            DECIMAL(18,2) = 0,
        @v_PersonasInclOrd      INT     = 4,
        @v_ValorPersAdicOrd     DECIMAL(18,2) = 0,
        @v_TarifaEsp            DECIMAL(18,2) = 0,
        @v_ValorPersAdicEsp     DECIMAL(18,2) = 0,
        @v_ExisteTarifaEsp      BIT     = 0,
        @v_PersonasAdicionales  INT     = 0,
        @v_ValorOrdinario       DECIMAL(18,2) = 0,
        @v_ValorEspecial        DECIMAL(18,2) = 0,
        @v_ValorPersonasOrd     DECIMAL(18,2) = 0,
        @v_ValorPersonasEsp     DECIMAL(18,2) = 0,
        @v_ValorLavanderia      DECIMAL(18,2) = 0,
        @v_ValorSubtotal        DECIMAL(18,2) = 0,
        @v_ValorTotal           DECIMAL(18,2) = 0,
        @v_NumHabTarifa         INT     = NULL,
        @v_FechaIter            DATE,
        @v_Dow                  INT;

    SET @v_NumeroNoches = DATEDIFF(DAY, @p_FechaLlegada, @p_FechaSalida);

    -- Obtener NumHabTarifa (del primer alojamiento o por número de habitaciones)
    SELECT TOP 1 @v_NumHabTarifa = NumeroHabitacionesTarifa
    FROM   dbo.Alojamientos
    WHERE  IdAlojamiento IN (SELECT IdAlojamiento FROM @v_Alojamientos);

    IF @v_NumHabTarifa IS NULL
        SET @v_NumHabTarifa = @p_NumeroHabitaciones;

    -- Tarifa ordinaria
    SELECT TOP 1
        @v_TarifaOrd        = t.TarifaBase,
        @v_PersonasInclOrd  = t.PersonasIncluidas,
        @v_ValorPersAdicOrd = t.ValorPersonaAdicional
    FROM   dbo.Tarifas t
    INNER JOIN dbo.Temporadas temp ON t.IdTemporada = temp.IdTemporada
    WHERE  t.Activo = 1
    AND    t.EsTarifaEspecial = 0
    AND    temp.EsTemporadaAlta = 0
    AND  (
        (t.IdSede = @p_IdSede AND t.IdAlojamiento IS NULL)
        OR (t.IdSede IS NULL AND t.IdAlojamiento IS NULL
            AND t.NumeroHabitacionesTarifa = @v_NumHabTarifa)
    )
    ORDER BY CASE WHEN t.IdSede IS NOT NULL THEN 1 ELSE 2 END;

    -- Tarifa especial Lun-Jue
    SELECT TOP 1
        @v_TarifaEsp        = t.TarifaBase,
        @v_ValorPersAdicEsp = t.ValorPersonaAdicional,
        @v_ExisteTarifaEsp  = 1
    FROM   dbo.Tarifas t
    INNER JOIN dbo.Temporadas temp ON t.IdTemporada = temp.IdTemporada
    WHERE  t.Activo = 1
    AND    t.EsTarifaEspecial = 1
    AND    temp.EsTarifaEspecial = 1
    AND  (
        (t.IdSede = @p_IdSede AND t.IdAlojamiento IS NULL)
        OR (t.IdSede IS NULL AND t.IdAlojamiento IS NULL
            AND t.NumeroHabitacionesTarifa = @v_NumHabTarifa)
    )
    ORDER BY CASE WHEN t.IdSede IS NOT NULL THEN 1 ELSE 2 END;

    -- Contar días ordinarios / especiales
    IF @v_ExisteTarifaEsp = 1
    BEGIN
        SET @v_FechaIter = @p_FechaLlegada;
        WHILE @v_FechaIter < @p_FechaSalida
        BEGIN
            SET @v_Dow = DATEPART(dw, @v_FechaIter);
            IF @v_Dow IN (2,3,4,5)
               AND NOT EXISTS (SELECT 1 FROM dbo.Festivos WHERE Fecha = @v_FechaIter AND Activo = 1)
                SET @v_DiasEspeciales = @v_DiasEspeciales + 1;
            ELSE
                SET @v_DiasOrdinarios = @v_DiasOrdinarios + 1;
            SET @v_FechaIter = DATEADD(DAY,1,@v_FechaIter);
        END;
    END
    ELSE
        SET @v_DiasOrdinarios = @v_NumeroNoches;

    -- Cálculo de valores
    SET @v_PersonasAdicionales = CASE WHEN @p_NumeroPersonas > @v_PersonasInclOrd
                                      THEN @p_NumeroPersonas - @v_PersonasInclOrd ELSE 0 END;
    SET @v_ValorOrdinario   = @v_DiasOrdinarios * @v_TarifaOrd * @p_NumeroHabitaciones;
    SET @v_ValorPersonasOrd = @v_DiasOrdinarios * @v_PersonasAdicionales * @v_ValorPersAdicOrd;
    SET @v_ValorEspecial    = @v_DiasEspeciales * @v_TarifaEsp * @p_NumeroHabitaciones;
    SET @v_ValorPersonasEsp = @v_DiasEspeciales * @v_PersonasAdicionales * @v_ValorPersAdicEsp;

    IF @p_IncluyeLavanderia = 1
        SELECT @v_ValorLavanderia = Valor FROM dbo.ServiciosAdicionales
        WHERE  Nombre = 'Servicio de Lavandería' AND Activo = 1;

    SET @v_ValorSubtotal = @v_ValorOrdinario + @v_ValorPersonasOrd
                         + @v_ValorEspecial  + @v_ValorPersonasEsp;
    SET @v_ValorTotal    = @v_ValorSubtotal + ISNULL(@v_ValorLavanderia, 0);

    -- -------------------------------------------------------
    -- Insertar la reserva en transacción
    -- -------------------------------------------------------
    DECLARE @v_IdReserva INT;
    DECLARE @v_CodigoReserva NVARCHAR(30);

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Generar código único
        SET @v_CodigoReserva = 'RES-'
            + CAST(YEAR(GETDATE()) AS NVARCHAR)
            + '-'
            + RIGHT('000000' + CAST(
                ISNULL((SELECT MAX(IdReserva) FROM dbo.Reservas), 0) + 1
              AS NVARCHAR), 6);

        INSERT INTO dbo.Reservas (
            UserId, IdSede, IdEstadoReserva, CodigoReserva,
            FechaLlegada, FechaSalida, NumeroNoches,
            NumeroPersonas, NumeroHabitaciones,
            ValorTarifaOrdinaria, ValorTarifaEspecial,
            ValorPersonasAdicionales, ValorLavanderia,
            ValorSubtotal, ValorTotal,
            IncluyeLavanderia, DiasOrdinarios, DiasEspeciales,
            Observaciones, UsuarioCreacion
        )
        VALUES (
            @p_UserId, @p_IdSede, 1, @v_CodigoReserva,
            @p_FechaLlegada, @p_FechaSalida, @v_NumeroNoches,
            @p_NumeroPersonas, @p_NumeroHabitaciones,
            @v_ValorOrdinario   + @v_ValorPersonasOrd,
            @v_ValorEspecial    + @v_ValorPersonasEsp,
            @v_PersonasAdicionales * (@v_ValorPersAdicOrd * @v_DiasOrdinarios
                                    + @v_ValorPersAdicEsp * @v_DiasEspeciales),
            ISNULL(@v_ValorLavanderia, 0),
            @v_ValorSubtotal, @v_ValorTotal,
            @p_IncluyeLavanderia, @v_DiasOrdinarios, @v_DiasEspeciales,
            @p_Observaciones, @p_UserId
        );

        SET @v_IdReserva = SCOPE_IDENTITY();

        -- Insertar detalle de alojamientos
        INSERT INTO dbo.ReservaDetalles (IdReserva, IdAlojamiento, ValorLinea)
        SELECT @v_IdReserva, va.IdAlojamiento, @v_ValorTotal / COUNT(*) OVER ()
        FROM   @v_Alojamientos va;

        -- Insertar lavandería si aplica
        IF @p_IncluyeLavanderia = 1 AND @v_ValorLavanderia > 0
        BEGIN
            DECLARE @v_IdServLav INT;
            SELECT @v_IdServLav = IdServicio FROM dbo.ServiciosAdicionales
            WHERE Nombre = 'Servicio de Lavandería' AND Activo = 1;

            INSERT INTO dbo.ReservaServiciosAdicionales
                (IdReserva, IdServicio, Cantidad, ValorUnitario, ValorTotal)
            VALUES
                (@v_IdReserva, @v_IdServLav, 1, @v_ValorLavanderia, @v_ValorLavanderia);
        END;

        COMMIT TRANSACTION;

        -- Retornar datos de la reserva creada
        SELECT
            r.IdReserva,
            r.CodigoReserva,
            s.Nombre        AS NombreSede,
            r.FechaReserva,
            r.FechaLlegada,
            r.FechaSalida,
            r.NumeroNoches,
            r.NumeroPersonas,
            r.NumeroHabitaciones,
            r.DiasOrdinarios,
            r.DiasEspeciales,
            r.ValorTarifaOrdinaria,
            r.ValorTarifaEspecial,
            r.ValorLavanderia,
            r.ValorSubtotal,
            r.ValorTotal,
            er.Nombre       AS EstadoReserva
        FROM   dbo.Reservas r
        INNER JOIN dbo.Sedes s           ON r.IdSede = s.IdSede
        INNER JOIN dbo.EstadosReserva er ON r.IdEstadoReserva = er.IdEstadoReserva
        WHERE  r.IdReserva = @v_IdReserva;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        DECLARE @v_MsgError NVARCHAR(2000) = ERROR_MESSAGE();
        RAISERROR(@v_MsgError, 16, 1);
    END CATCH;
END;
GO

PRINT 'SP sp_CrearReserva creado.';

-- ============================================================
-- 6. sp_ConsultarReservasUsuario
--    Historial de reservas del usuario autenticado.
-- ============================================================
IF OBJECT_ID('dbo.sp_ConsultarReservasUsuario', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ConsultarReservasUsuario;
GO

CREATE PROCEDURE dbo.sp_ConsultarReservasUsuario
    @p_UserId       NVARCHAR(450),
    @p_SoloActivas  BIT = 0        -- 1 = excluye canceladas
AS
BEGIN
    SET NOCOUNT ON;

    IF @p_UserId IS NULL OR LEN(TRIM(@p_UserId)) = 0
    BEGIN
        RAISERROR('El usuario es obligatorio.', 16, 1); RETURN;
    END

    SELECT
        r.IdReserva,
        r.CodigoReserva,
        s.Nombre                        AS Lugar,
        ts.Nombre                       AS TipoLugar,
        m.Nombre                        AS Ciudad,
        r.FechaReserva,
        r.FechaLlegada,
        r.FechaSalida,
        r.NumeroNoches,
        r.NumeroPersonas                AS [No. Personas],
        r.NumeroHabitaciones            AS [No. Habitaciones],
        r.DiasOrdinarios,
        r.DiasEspeciales,
        r.ValorTarifaOrdinaria,
        r.ValorTarifaEspecial,
        r.ValorLavanderia,
        r.ValorSubtotal,
        r.ValorTotal                    AS ValorTotal,
        er.Nombre                       AS EstadoReserva,
        r.IncluyeLavanderia,
        r.Observaciones,
        -- Alojamientos en la reserva (concatenados)
        (
            SELECT STRING_AGG(a.Nombre, ', ')
            FROM   dbo.ReservaDetalles rd2
            INNER JOIN dbo.Alojamientos a ON rd2.IdAlojamiento = a.IdAlojamiento
            WHERE  rd2.IdReserva = r.IdReserva
        )                               AS Alojamientos
    FROM   dbo.Reservas r
    INNER JOIN dbo.Sedes s              ON r.IdSede = s.IdSede
    INNER JOIN dbo.TiposSede ts         ON s.IdTipoSede = ts.IdTipoSede
    INNER JOIN dbo.Municipios m         ON s.IdMunicipio = m.IdMunicipio
    INNER JOIN dbo.EstadosReserva er    ON r.IdEstadoReserva = er.IdEstadoReserva
    WHERE  r.UserId = @p_UserId
    AND    r.Activo = 1
    AND    (@p_SoloActivas = 0 OR er.Nombre != 'Cancelada')
    ORDER BY r.FechaReserva DESC;
END;
GO

PRINT 'SP sp_ConsultarReservasUsuario creado.';

-- ============================================================
-- 7. sp_CancelarReserva
--    Cancela una reserva existente del usuario.
-- ============================================================
IF OBJECT_ID('dbo.sp_CancelarReserva', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CancelarReserva;
GO

CREATE PROCEDURE dbo.sp_CancelarReserva
    @p_IdReserva    INT,
    @p_UserId       NVARCHAR(450),
    @p_Motivo       NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_IdEstadoCancelada INT;
    SELECT @v_IdEstadoCancelada = IdEstadoReserva
    FROM   dbo.EstadosReserva
    WHERE  Nombre = 'Cancelada';

    DECLARE @v_EstadoActual NVARCHAR(50);
    SELECT @v_EstadoActual = er.Nombre
    FROM   dbo.Reservas r
    INNER JOIN dbo.EstadosReserva er ON r.IdEstadoReserva = er.IdEstadoReserva
    WHERE  r.IdReserva = @p_IdReserva
    AND    r.UserId    = @p_UserId;

    IF @v_EstadoActual IS NULL
    BEGIN
        RAISERROR('Reserva no encontrada o no pertenece al usuario.', 16, 1); RETURN;
    END

    IF @v_EstadoActual = 'Cancelada'
    BEGIN
        RAISERROR('La reserva ya está cancelada.', 16, 1); RETURN;
    END

    IF @v_EstadoActual = 'Pagada'
    BEGIN
        RAISERROR('No se puede cancelar una reserva pagada sin autorización administrativa.', 16, 1); RETURN;
    END

    UPDATE dbo.Reservas
    SET    IdEstadoReserva     = @v_IdEstadoCancelada,
           Observaciones       = ISNULL(@p_Motivo, Observaciones),
           FechaModificacion   = GETDATE(),
           UsuarioModificacion = @p_UserId
    WHERE  IdReserva = @p_IdReserva;

    SELECT 'Reserva cancelada correctamente.' AS Mensaje, @p_IdReserva AS IdReserva;
END;
GO

PRINT 'SP sp_CancelarReserva creado.';

-- ============================================================
-- 8. sp_ConsultarCalendarioDisponibilidad
--    Devuelve disponibilidad de un alojamiento mes a mes.
-- ============================================================
IF OBJECT_ID('dbo.sp_ConsultarCalendarioDisponibilidad', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ConsultarCalendarioDisponibilidad;
GO

CREATE PROCEDURE dbo.sp_ConsultarCalendarioDisponibilidad
    @p_IdAlojamiento    INT,
    @p_Anio             INT = NULL,   -- NULL = año actual
    @p_Mes              INT = NULL    -- NULL = mes actual
AS
BEGIN
    SET NOCOUNT ON;

    IF @p_Anio IS NULL  SET @p_Anio  = YEAR(GETDATE());
    IF @p_Mes  IS NULL  SET @p_Mes   = MONTH(GETDATE());

    DECLARE @v_FechaInicio DATE = DATEFROMPARTS(@p_Anio, @p_Mes, 1);
    DECLARE @v_FechaFin    DATE = EOMONTH(@v_FechaInicio);

    -- Generar todos los días del mes
    ;WITH Dias AS (
        SELECT @v_FechaInicio AS Fecha
        UNION ALL
        SELECT DATEADD(DAY, 1, Fecha)
        FROM   Dias
        WHERE  Fecha < @v_FechaFin
    )
    SELECT
        d.Fecha,
        DATENAME(WEEKDAY, d.Fecha)  AS DiaSemana,
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM   dbo.ReservaDetalles rd
                INNER JOIN dbo.Reservas r  ON rd.IdReserva = r.IdReserva
                INNER JOIN dbo.EstadosReserva er ON r.IdEstadoReserva = er.IdEstadoReserva
                WHERE  rd.IdAlojamiento = @p_IdAlojamiento
                AND    er.BloqueoDisponibilidad = 1
                AND    d.Fecha >= r.FechaLlegada
                AND    d.Fecha <  r.FechaSalida
            )
            THEN 'Ocupado'
            ELSE 'Disponible'
        END                         AS Estado,
        CASE
            WHEN EXISTS (SELECT 1 FROM dbo.Festivos WHERE Fecha = d.Fecha AND Activo = 1)
            THEN 1 ELSE 0
        END                         AS EsFestivo
    FROM Dias d
    OPTION (MAXRECURSION 31);
END;
GO

PRINT 'SP sp_ConsultarCalendarioDisponibilidad creado.';

PRINT '=== Todos los procedimientos almacenados creados correctamente. ===';
GO
