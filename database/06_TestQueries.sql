-- ============================================================
-- DB_ReservasFodun
-- Sistema de Reservas - Sedes Recreativas y Apartamentos FODUN
-- ============================================================
-- Script  : 06_TestQueries.sql
-- Autor   : Arquitecto BD Senior
-- Fecha   : 2026
-- Desc    : Consultas de prueba para validar el modelo,
--           los datos y los procedimientos almacenados.
-- ============================================================
-- EJECUTAR SECCIÓN POR SECCIÓN para verificar resultados.
-- ============================================================

USE DB_ReservasFodun;
GO

SET NOCOUNT ON;

-- ============================================================
-- BLOQUE 1: VERIFICACIÓN DE DATOS MAESTROS
-- ============================================================
PRINT '=== BLOQUE 1: DATOS MAESTROS ===';

-- 1.1 Resumen general de tablas
SELECT 'Departamentos'         AS Tabla, COUNT(*) AS Total FROM dbo.Departamentos
UNION ALL SELECT 'Municipios',           COUNT(*) FROM dbo.Municipios
UNION ALL SELECT 'TiposSede',            COUNT(*) FROM dbo.TiposSede
UNION ALL SELECT 'TiposAlojamiento',     COUNT(*) FROM dbo.TiposAlojamiento
UNION ALL SELECT 'Caracteristicas',      COUNT(*) FROM dbo.Caracteristicas
UNION ALL SELECT 'Sedes',                COUNT(*) FROM dbo.Sedes
UNION ALL SELECT 'Alojamientos',         COUNT(*) FROM dbo.Alojamientos
UNION ALL SELECT 'Temporadas',           COUNT(*) FROM dbo.Temporadas
UNION ALL SELECT 'Tarifas',              COUNT(*) FROM dbo.Tarifas
UNION ALL SELECT 'EstadosReserva',       COUNT(*) FROM dbo.EstadosReserva
UNION ALL SELECT 'ServiciosAdicionales', COUNT(*) FROM dbo.ServiciosAdicionales
UNION ALL SELECT 'Festivos',             COUNT(*) FROM dbo.Festivos;
GO

-- 1.2 Sedes con tipo y ubicación
SELECT
    s.IdSede,
    ts.Nombre       AS TipoSede,
    s.Nombre        AS NombreSede,
    m.Nombre        AS Ciudad,
    d.Nombre        AS Departamento,
    s.CapacidadTotal
FROM dbo.Sedes s
INNER JOIN dbo.TiposSede ts ON s.IdTipoSede = ts.IdTipoSede
INNER JOIN dbo.Municipios m ON s.IdMunicipio = m.IdMunicipio
INNER JOIN dbo.Departamentos d ON m.IdDepartamento = d.IdDepartamento
ORDER BY ts.Nombre, s.Nombre;
GO

-- 1.3 Alojamientos por sede con tarifa aplicable
SELECT
    s.Nombre        AS Sede,
    a.IdAlojamiento,
    a.Numero,
    a.Nombre        AS Alojamiento,
    ta.Nombre       AS TipoAlojamiento,
    a.NumeroHabitaciones,
    a.CapacidadMaxima,
    a.NumeroHabitacionesTarifa,
    CASE
        WHEN a.NumeroHabitacionesTarifa = 1 THEN '$70.000 / $27.000'
        WHEN a.NumeroHabitacionesTarifa = 2 THEN '$90.000 / $37.000'
        ELSE 'Tarifa propia'
    END             AS TarifaOrdinaria_Especial
FROM dbo.Alojamientos a
INNER JOIN dbo.Sedes s ON a.IdSede = s.IdSede
INNER JOIN dbo.TiposAlojamiento ta ON a.IdTipoAlojamiento = ta.IdTipoAlojamiento
WHERE a.Activo = 1
ORDER BY s.IdSede, CAST(a.Numero AS INT);
GO

-- 1.4 Tarifas registradas con detalle
SELECT
    t.IdTarifa,
    ISNULL(s.Nombre, '(Todas las sedes recreativas)') AS Sede,
    ISNULL(a.Nombre, '(Por categoría)')               AS Alojamiento,
    temp.Nombre     AS Temporada,
    t.NumeroHabitacionesTarifa                         AS NumHabTarifa,
    t.PersonasIncluidas,
    FORMAT(t.TarifaBase, 'N0')                        AS TarifaBase,
    FORMAT(t.ValorPersonaAdicional, 'N0')              AS ValorPersAdicional,
    CASE t.EsTarifaEspecial WHEN 1 THEN 'Especial Lun-Jue' ELSE 'Ordinaria/Normal' END AS TipoTarifa
FROM dbo.Tarifas t
INNER JOIN dbo.Temporadas temp ON t.IdTemporada = temp.IdTemporada
LEFT  JOIN dbo.Sedes s         ON t.IdSede = s.IdSede
LEFT  JOIN dbo.Alojamientos a  ON t.IdAlojamiento = a.IdAlojamiento
WHERE t.Activo = 1
ORDER BY s.Nombre, a.Nombre, temp.Nombre, t.EsTarifaEspecial;
GO

-- ============================================================
-- BLOQUE 2: sp_ConsultarAlojamientosDisponiblesPorFecha
-- ============================================================
PRINT '=== BLOQUE 2: DISPONIBILIDAD POR FECHA ===';

-- 2.1 Todos los alojamientos disponibles (sin reservas aún)
EXEC dbo.sp_ConsultarAlojamientosDisponiblesPorFecha
    @p_FechaLlegada = '2026-08-10',
    @p_FechaSalida  = '2026-08-13';
GO

-- 2.2 Solo alojamientos de la Sede Villeta (IdSede=1)
EXEC dbo.sp_ConsultarAlojamientosDisponiblesPorFecha
    @p_FechaLlegada = '2026-08-10',
    @p_FechaSalida  = '2026-08-13',
    @p_IdSede       = 1;
GO

-- 2.3 Solo alojamientos del Edificio Reina 1 - Santa Marta (IdSede=8)
EXEC dbo.sp_ConsultarAlojamientosDisponiblesPorFecha
    @p_FechaLlegada = '2026-12-20',
    @p_FechaSalida  = '2026-12-27',
    @p_IdSede       = 8;
GO

-- ============================================================
-- BLOQUE 3: sp_ConsultarAlojamientosDisponiblesPorFechaPersonas
-- ============================================================
PRINT '=== BLOQUE 3: DISPONIBILIDAD POR FECHA Y PERSONAS ===';

-- 3.1 Buscar para 4 personas en Villeta
EXEC dbo.sp_ConsultarAlojamientosDisponiblesPorFechaPersonas
    @p_FechaLlegada   = '2026-08-10',
    @p_FechaSalida    = '2026-08-13',
    @p_NumeroPersonas = 4,
    @p_IdSede         = 1;
GO

-- 3.2 Buscar para 6 personas en Santa Marta (debe sugerir Apto 202 u 8-personas)
EXEC dbo.sp_ConsultarAlojamientosDisponiblesPorFechaPersonas
    @p_FechaLlegada   = '2026-12-20',
    @p_FechaSalida    = '2026-12-27',
    @p_NumeroPersonas = 6,
    @p_IdSede         = 8;
GO

-- 3.3 Buscar para 2 personas en Suramericana Medellín (IdSede=7)
EXEC dbo.sp_ConsultarAlojamientosDisponiblesPorFechaPersonas
    @p_FechaLlegada   = '2026-09-01',
    @p_FechaSalida    = '2026-09-04',
    @p_NumeroPersonas = 2,
    @p_IdSede         = 7;
GO

-- ============================================================
-- BLOQUE 4: sp_ConsultarTarifas
-- ============================================================
PRINT '=== BLOQUE 4: CONSULTA DE TARIFAS ===';

-- 4.1 Tarifas de Villeta (IdSede=1), habitación 1 (IdAlojamiento=1), para 5 personas
-- Debe mostrar tarifa ordinaria ($70K) y especial ($27K), con 1 persona adicional
EXEC dbo.sp_ConsultarTarifas
    @p_IdSede        = 1,
    @p_IdAlojamiento = 1,
    @p_NumeroPersonas = 5;
GO

-- 4.2 Tarifas de Apartamento 301 Santa Marta (IdAlojamiento=48), baja y alta temporada
EXEC dbo.sp_ConsultarTarifas
    @p_IdSede        = 8,
    @p_IdAlojamiento = 48,
    @p_NumeroPersonas = 4;
GO

-- 4.3 Tarifas de Apartamento 202 Santa Marta en Alta Temporada (IdTemporada=2)
EXEC dbo.sp_ConsultarTarifas
    @p_IdSede        = 8,
    @p_IdAlojamiento = 47,
    @p_IdTemporada   = 2,
    @p_NumeroPersonas = 8;
GO

-- 4.4 Tarifas de Suramericana Medellín (IdSede=7), 1 persona
EXEC dbo.sp_ConsultarTarifas
    @p_IdSede        = 7,
    @p_NumeroPersonas = 1;
GO

-- 4.5 Tarifas de El Placer (IdSede=2), para alojamiento 2 habitaciones
EXEC dbo.sp_ConsultarTarifas
    @p_IdSede        = 2,
    @p_IdAlojamiento = 9,  -- Alojamiento 1 El Placer (2 habitaciones)
    @p_NumeroPersonas = 6;
GO

-- ============================================================
-- BLOQUE 5: sp_CalcularValorReserva
-- ============================================================
PRINT '=== BLOQUE 5: CÁLCULO DE VALOR DE RESERVA ===';

-- 5.1 CASO: Villeta - 1 habitación - 4 personas - 3 noches
-- Lunes 10 a Jueves 13 de agosto 2026 (3 noches, todas especiales Lun-Jue)
-- Valor esperado: 3 * $27.000 = $81.000
PRINT '--- 5.1 Villeta 1 hab, 4 personas, Lun 10 a Jue 13 ago (3 días especiales)';
EXEC dbo.sp_CalcularValorReserva
    @p_IdSede             = 1,
    @p_IdAlojamiento      = 1,
    @p_NumeroHabitaciones = 1,
    @p_NumeroPersonas     = 4,
    @p_FechaLlegada       = '2026-08-10',
    @p_FechaSalida        = '2026-08-13',
    @p_IncluyeLavanderia  = 0;
GO

-- 5.2 CASO: El Placer - 2 habitaciones - 6 personas - Viernes a Lunes (3 noches)
-- Sáb 15 + Dom 16 = ordinarios ($90K c/u), Lun 17 = especial ($37K)
-- + 2 personas adicionales
PRINT '--- 5.2 El Placer 2 hab, 6 personas, Vie 14 a Lun 17 ago (2 ord + 1 esp)';
EXEC dbo.sp_CalcularValorReserva
    @p_IdSede             = 2,
    @p_IdAlojamiento      = 9,   -- Alojamiento 1, 2 hab
    @p_NumeroHabitaciones = 2,
    @p_NumeroPersonas     = 6,
    @p_FechaLlegada       = '2026-08-14',
    @p_FechaSalida        = '2026-08-17',
    @p_IncluyeLavanderia  = 0;
GO

-- 5.3 CASO: Santa Marta Apto 301 (IdAlojamiento=48) - Baja temporada
-- 5 noches, 6 personas: $89.000/noche * 5 = $445.000
PRINT '--- 5.3 Apto 301 Santa Marta, 6 personas, baja temporada, 5 noches';
EXEC dbo.sp_CalcularValorReserva
    @p_IdSede             = 8,
    @p_IdAlojamiento      = 48,
    @p_NumeroHabitaciones = 1,
    @p_NumeroPersonas     = 6,
    @p_FechaLlegada       = '2026-09-05',
    @p_FechaSalida        = '2026-09-10',
    @p_IncluyeLavanderia  = 1;   -- Con lavandería: + $18.000
GO

-- 5.4 CASO: Santa Marta Apto 202 (IdAlojamiento=47) - Alta temporada
-- 7 noches (Navidad), 8 personas: $143.000/noche * 7 = $1.001.000
PRINT '--- 5.4 Apto 202 Santa Marta, 8 personas, alta temporada (Navidad)';
EXEC dbo.sp_CalcularValorReserva
    @p_IdSede             = 8,
    @p_IdAlojamiento      = 47,
    @p_NumeroHabitaciones = 1,
    @p_NumeroPersonas     = 8,
    @p_FechaLlegada       = '2026-12-20',
    @p_FechaSalida        = '2026-12-27',
    @p_IncluyeLavanderia  = 1;
GO

-- 5.5 CASO: Suramericana Medellín - 1 habitación - 2 personas - 3 noches
-- $75.000/noche * 3 = $225.000
PRINT '--- 5.5 Suramericana Medellín, 2 personas, 3 noches';
EXEC dbo.sp_CalcularValorReserva
    @p_IdSede             = 7,
    @p_IdAlojamiento      = 42,  -- Habitación 1 Suramericana
    @p_NumeroHabitaciones = 1,
    @p_NumeroPersonas     = 2,
    @p_FechaLlegada       = '2026-09-10',
    @p_FechaSalida        = '2026-09-13',
    @p_IncluyeLavanderia  = 0;
GO

-- ============================================================
-- BLOQUE 6: SIMULACIÓN COMPLETA DE RESERVA + CONSULTA
-- ============================================================
PRINT '=== BLOQUE 6: SIMULACIÓN DE RESERVA ===';

-- 6.1 Crear una reserva de prueba manualmente (sin SP)
-- Usuario ficticio (en producción sería el UserId de AspNetUsers)
DECLARE @v_UserIdPrueba NVARCHAR(450) = 'user-prueba-00000001';

INSERT INTO dbo.Reservas (
    UserId, IdSede, IdEstadoReserva, CodigoReserva,
    FechaLlegada, FechaSalida, NumeroNoches,
    NumeroPersonas, NumeroHabitaciones,
    ValorTarifaOrdinaria, ValorTarifaEspecial,
    ValorPersonasAdicionales, ValorLavanderia, ValorOtrosServicios,
    ValorSubtotal, ValorTotal,
    IncluyeLavanderia, DiasOrdinarios, DiasEspeciales,
    Observaciones, UsuarioCreacion
)
VALUES (
    @v_UserIdPrueba, 1, 2, 'RES-2026-000001',
    '2026-08-10', '2026-08-13', 3,
    4, 1,
    0, 81000,    -- 3 días especiales * $27.000
    0, 0, 0,
    81000, 81000,
    0, 0, 3,
    'Reserva de prueba para validación', @v_UserIdPrueba
);

-- Insertar detalle del alojamiento reservado (Habitación 1, Villeta)
DECLARE @v_IdReservaPrueba INT = SCOPE_IDENTITY();

INSERT INTO dbo.ReservaDetalles (IdReserva, IdAlojamiento, ValorLinea)
VALUES (@v_IdReservaPrueba, 1, 81000);

PRINT 'Reserva de prueba creada con IdReserva = ' + CAST(@v_IdReservaPrueba AS NVARCHAR);
GO

-- 6.2 Verificar que la habitación 1 de Villeta aparece como NO disponible
-- en las mismas fechas
PRINT '--- Disponibilidad habitación 1 Villeta en fechas reservadas (debe ser NO disponible):';
EXEC dbo.sp_ConsultarAlojamientosDisponiblesPorFecha
    @p_FechaLlegada = '2026-08-10',
    @p_FechaSalida  = '2026-08-13',
    @p_IdSede       = 1;
GO

-- 6.3 Verificar disponibilidad en fechas distintas (no debe haber conflicto)
PRINT '--- Disponibilidad habitación 1 Villeta en otras fechas (debe ser disponible):';
EXEC dbo.sp_ConsultarAlojamientosDisponiblesPorFecha
    @p_FechaLlegada = '2026-08-14',
    @p_FechaSalida  = '2026-08-17',
    @p_IdSede       = 1;
GO

-- 6.4 Consultar reservas del usuario de prueba
PRINT '--- Reservas del usuario de prueba:';
EXEC dbo.sp_ConsultarReservasUsuario
    @p_UserId      = 'user-prueba-00000001',
    @p_SoloActivas = 0;
GO

-- 6.5 Cancelar la reserva de prueba
PRINT '--- Cancelar la reserva de prueba:';
DECLARE @v_IdCancelar INT;
SELECT @v_IdCancelar = IdReserva FROM dbo.Reservas
WHERE CodigoReserva = 'RES-2026-000001';

EXEC dbo.sp_CancelarReserva
    @p_IdReserva = @v_IdCancelar,
    @p_UserId    = 'user-prueba-00000001',
    @p_Motivo    = 'Cancelación de prueba técnica';
GO

-- 6.6 Verificar que la habitación vuelve a estar disponible tras cancelación
PRINT '--- Disponibilidad habitación 1 tras cancelar reserva (debe ser disponible):';
EXEC dbo.sp_ConsultarAlojamientosDisponiblesPorFecha
    @p_FechaLlegada = '2026-08-10',
    @p_FechaSalida  = '2026-08-13',
    @p_IdSede       = 1;
GO

-- ============================================================
-- BLOQUE 7: CONSULTA DE CALENDARIO DE DISPONIBILIDAD
-- ============================================================
PRINT '=== BLOQUE 7: CALENDARIO DE DISPONIBILIDAD ===';

-- 7.1 Calendario de agosto 2026 para Habitación 1 de Villeta (IdAlojamiento=1)
EXEC dbo.sp_ConsultarCalendarioDisponibilidad
    @p_IdAlojamiento = 1,
    @p_Anio          = 2026,
    @p_Mes           = 8;
GO

-- 7.2 Calendario de diciembre 2026 para Apto 301 Santa Marta (IdAlojamiento=48)
EXEC dbo.sp_ConsultarCalendarioDisponibilidad
    @p_IdAlojamiento = 48,
    @p_Anio          = 2026,
    @p_Mes           = 12;
GO

-- ============================================================
-- BLOQUE 8: CONSULTAS DE VALIDACIÓN DEL MODELO
-- ============================================================
PRINT '=== BLOQUE 8: VALIDACIÓN DEL MODELO RELACIONAL ===';

-- 8.1 ¿Cuántos alojamientos por sede y capacidad total?
SELECT
    s.Nombre        AS Sede,
    ts.Nombre       AS TipoSede,
    COUNT(a.IdAlojamiento)  AS TotalAlojamientos,
    SUM(a.CapacidadMaxima)  AS CapacidadSumada,
    s.CapacidadTotal        AS CapacidadDeclarada
FROM dbo.Sedes s
INNER JOIN dbo.TiposSede ts ON s.IdTipoSede = ts.IdTipoSede
LEFT  JOIN dbo.Alojamientos a ON s.IdSede = a.IdSede AND a.Activo = 1
GROUP BY s.IdSede, s.Nombre, ts.Nombre, s.CapacidadTotal
ORDER BY ts.Nombre, s.Nombre;
GO

-- 8.2 ¿Las tarifas cubren todas las sedes y alojamientos esperados?
SELECT
    'Tarifas genéricas (sedes recreativas)' AS Tipo,
    COUNT(*) AS Total
FROM dbo.Tarifas
WHERE IdSede IS NULL AND IdAlojamiento IS NULL

UNION ALL

SELECT 'Tarifas Suramericana Medellín', COUNT(*)
FROM dbo.Tarifas
WHERE IdSede = 7

UNION ALL

SELECT 'Tarifas Apto 202 Santa Marta', COUNT(*)
FROM dbo.Tarifas
WHERE IdAlojamiento = 47

UNION ALL

SELECT 'Tarifas Apto 301 Santa Marta', COUNT(*)
FROM dbo.Tarifas
WHERE IdAlojamiento = 48

UNION ALL

SELECT 'Tarifas Apto 401 Santa Marta', COUNT(*)
FROM dbo.Tarifas
WHERE IdAlojamiento = 49;
GO

-- 8.3 Estados de reserva y su efecto en disponibilidad
SELECT
    Nombre,
    Descripcion,
    CASE BloqueoDisponibilidad WHEN 1 THEN 'SÍ bloquea' ELSE 'NO bloquea' END AS EfectoDisponibilidad
FROM dbo.EstadosReserva;
GO

-- 8.4 Resumen de festivos registrados para 2026
SELECT
    MONTH(Fecha) AS Mes,
    DATENAME(MONTH, Fecha) AS NombreMes,
    COUNT(*) AS TotalFestivos
FROM dbo.Festivos
WHERE YEAR(Fecha) = 2026 AND Activo = 1
GROUP BY MONTH(Fecha), DATENAME(MONTH, Fecha)
ORDER BY Mes;
GO

-- ============================================================
-- BLOQUE 9: LIMPIAR DATOS DE PRUEBA
-- ============================================================
PRINT '=== BLOQUE 9: LIMPIEZA DE DATOS DE PRUEBA ===';

-- Eliminar registros de prueba (si no se quieren conservar)
DELETE FROM dbo.ReservaServiciosAdicionales
WHERE IdReserva IN (
    SELECT IdReserva FROM dbo.Reservas WHERE UserId = 'user-prueba-00000001'
);

DELETE FROM dbo.ReservaDetalles
WHERE IdReserva IN (
    SELECT IdReserva FROM dbo.Reservas WHERE UserId = 'user-prueba-00000001'
);

DELETE FROM dbo.Reservas WHERE UserId = 'user-prueba-00000001';

PRINT 'Datos de prueba eliminados.';
GO

-- ============================================================
-- RESULTADO FINAL ESPERADO
-- ============================================================
-- sp_ConsultarAlojamientosDisponiblesPorFecha:
--   Bloque 2.1 → 49 alojamientos disponibles (sin reservas)
--   Bloque 6.2 → Habitación 1 Villeta aparece como NO disponible
--   Bloque 6.3 → Todas disponibles en otras fechas
--   Bloque 6.6 → Habitación 1 vuelve a estar disponible (cancelada)
--
-- sp_ConsultarTarifas:
--   Bloque 4.1 → Ordinaria $70K y Especial $27K (1 persona adicional)
--   Bloque 4.2 → Baja $89K y Alta $124K para Apto 301
--   Bloque 4.4 → $63K (1 persona) y $75K (2 personas) para Suramericana
--
-- sp_CalcularValorReserva:
--   Bloque 5.1 → $81.000 (3 noches especiales * $27K)
--   Bloque 5.3 → $463.000 ($89K*5 + $18K lavandería)
--   Bloque 5.4 → $1.019.000 ($143K*7 + $18K lavandería)
-- ============================================================

PRINT '=== 06_TestQueries.sql ejecutado correctamente. ===';
GO
