-- ============================================================
-- DB_ReservasFodun
-- Sistema de Reservas - Sedes Recreativas y Apartamentos FODUN
-- ============================================================
-- Script  : 03_SeedData.sql
-- Autor   : Arquitecto BD Senior
-- Fecha   : 2026
-- Desc    : Datos iniciales: sedes, alojamientos, tarifas,
--           temporadas, estados, servicios y catálogos base.
-- ============================================================

USE DB_ReservasFodun;
GO

SET NOCOUNT ON;

-- ============================================================
-- DEPARTAMENTOS
-- ============================================================
INSERT INTO dbo.Departamentos (Nombre, CodigoDane) VALUES
    ('Cundinamarca',          '25'),  -- 1
    ('Antioquia',             '05'),  -- 2
    ('Caldas',                '17'),  -- 3
    ('Valle del Cauca',       '76'),  -- 4
    ('Magdalena',             '47'),  -- 5
    ('Bogotá D.C.',           '11');  -- 6
GO

PRINT 'Departamentos insertados.';

-- ============================================================
-- MUNICIPIOS
-- ============================================================
INSERT INTO dbo.Municipios (IdDepartamento, Nombre, CodigoDane) VALUES
    (1, 'Villeta',                '25843'),  -- 1
    (1, 'Fusagasugá',             '25290'),  -- 2
    (3, 'Chinchiná',              '17174'),  -- 3
    (4, 'Palmira',                '76520'),  -- 4
    (2, 'Santa Fe de Antioquia',  '05042'),  -- 5
    (6, 'Bogotá',                 '11001'),  -- 6
    (2, 'Medellín',               '05001'),  -- 7
    (5, 'Santa Marta',            '47001');  -- 8
GO

PRINT 'Municipios insertados.';

-- ============================================================
-- TIPOS DE SEDE
-- ============================================================
INSERT INTO dbo.TiposSede (Nombre, Descripcion) VALUES
    ('Sede Recreativa', 'Instalaciones recreativas con alojamientos tipo habitación o cabaña'),   -- 1
    ('Apartamento',     'Edificios con apartamentos o habitaciones en centros urbanos o turísticos'); -- 2
GO

PRINT 'TiposSede insertados.';

-- ============================================================
-- TIPOS DE ALOJAMIENTO
-- ============================================================
INSERT INTO dbo.TiposAlojamiento (Nombre, Descripcion) VALUES
    ('Habitación 1 Alcoba',     'Habitación simple con 1 alcoba'),                                -- 1
    ('Alojamiento 1 Habitación','Alojamiento independiente con 1 habitación'),                    -- 2
    ('Alojamiento 2 Habitaciones','Alojamiento independiente con 2 habitaciones'),                -- 3
    ('Cabaña 1 Habitación',     'Cabaña independiente con 1 habitación y sala'),                  -- 4
    ('Cabaña 2 Habitaciones',   'Cabaña independiente con 2 habitaciones'),                       -- 5
    ('Apartamento',             'Apartamento completo con sala, cocina, habitaciones y baños'),   -- 6
    ('Habitación Privada',      'Habitación con baño privado en edificio de apartamentos');       -- 7
GO

PRINT 'TiposAlojamiento insertados.';

-- ============================================================
-- CARACTERÍSTICAS
-- ============================================================
INSERT INTO dbo.Caracteristicas (Nombre, Icono) VALUES
    ('Cama doble',          'fa-bed'),
    ('Cama sencilla',       'fa-bed'),
    ('Camarote',            'fa-bed'),
    ('Sofá cama',           'fa-couch'),
    ('Cama gemela',         'fa-bed'),
    ('Baño privado',        'fa-bath'),
    ('Baño compartido',     'fa-bath'),
    ('Televisor',           'fa-tv'),
    ('Nevera',              'fa-snowflake'),
    ('Cocineta',            'fa-utensils'),
    ('Cocina equipada',     'fa-kitchen-set'),
    ('Terraza',             'fa-building'),
    ('Terraza cubierta',    'fa-umbrella'),
    ('Parqueadero',         'fa-parking'),
    ('Sala comedor',        'fa-chair'),
    ('Zona húmeda',         'fa-water'),
    ('Jacuzzi',             'fa-hot-tub'),
    ('Sauna',               'fa-temperature-high'),
    ('Gimnasio',            'fa-dumbbell'),
    ('Piscina',             'fa-swimming-pool'),
    ('WiFi',                'fa-wifi'),
    ('Aire acondicionado',  'fa-wind');
GO

PRINT 'Caracteristicas insertadas.';

-- ============================================================
-- TEMPORADAS
-- DiasAplicacion: valores DATEPART(dw) con SET DATEFIRST 7
-- 1=Dom, 2=Lun, 3=Mar, 4=Mié, 5=Jue, 6=Vie, 7=Sáb
-- ============================================================
INSERT INTO dbo.Temporadas (Nombre, Descripcion, EsTemporadaAlta, EsTarifaEspecial, DiasAplicacion) VALUES
    ('Ordinaria',       'Tarifa estándar para sedes recreativas. Aplica todos los días no especiales.',
                        0, 0, NULL),               -- 1: IdTemporada=1
    ('Alta',            'Temporada alta (Semana Santa, Diciembre, vacaciones escolares). Mayor demanda en apartamentos Santa Marta.',
                        1, 0, NULL),               -- 2: IdTemporada=2
    ('Especial Semana', 'Tarifa promocional Lunes a Jueves, excepto festivos, semana escolar y temporada alta.',
                        0, 1, '2,3,4,5'),          -- 3: IdTemporada=3 (Lun-Jue dw)
    ('Baja',            'Temporada baja en apartamentos Santa Marta. Equivalente a temporada ordinaria.',
                        0, 0, NULL);               -- 4: IdTemporada=4
GO

PRINT 'Temporadas insertadas.';

-- ============================================================
-- ESTADOS DE RESERVA
-- ============================================================
INSERT INTO dbo.EstadosReserva (Nombre, Descripcion, BloqueoDisponibilidad) VALUES
    ('Pendiente',   'Reserva creada, pendiente de confirmación o pago', 1),  -- 1
    ('Confirmada',  'Reserva confirmada por el sistema',                1),  -- 2
    ('Pagada',      'Reserva con pago registrado',                      1),  -- 3
    ('Cancelada',   'Reserva cancelada. No bloquea disponibilidad',     0);  -- 4
GO

PRINT 'EstadosReserva insertados.';

-- ============================================================
-- SERVICIOS ADICIONALES
-- ============================================================
INSERT INTO dbo.ServiciosAdicionales (Nombre, Descripcion, Valor, PorPersona, PorNoche) VALUES
    ('Servicio de Lavandería',
     'Servicio de lavado de ropa. Aplica en apartamentos Santa Marta.',
     18000, 0, 0),                                                  -- 1: fijo por reserva

    ('Visita Día Acompañante',
     'Tarifa por visita día para acompañantes a partir del quinto y hasta máximo el décimo. '
     + 'Aplica en Villeta, El Placer, Manguruma, Gonzalo Morante y Tablones.',
     5500, 1, 0);                                                    -- 2: por persona
GO

PRINT 'ServiciosAdicionales insertados.';

-- ============================================================
-- PREGUNTAS SECRETAS
-- ============================================================
INSERT INTO dbo.PreguntasSecretas (Pregunta) VALUES
    ('¿Cuál es el nombre de su primera mascota?'),
    ('¿Cuál es el nombre de su ciudad natal?'),
    ('¿Cuál es el nombre de su madre?'),
    ('¿Cuál es el nombre de su colegio?'),
    ('¿Cuál es su deporte favorito?');
GO

PRINT 'PreguntasSecretas insertadas.';

-- ============================================================
-- FESTIVOS COLOMBIA 2026 (muestra)
-- ============================================================
INSERT INTO dbo.Festivos (Fecha, Nombre) VALUES
    ('2026-01-01', 'Año Nuevo'),
    ('2026-01-12', 'Día de los Reyes Magos'),
    ('2026-03-23', 'Día de San José'),
    ('2026-04-02', 'Jueves Santo'),
    ('2026-04-03', 'Viernes Santo'),
    ('2026-05-01', 'Día del Trabajo'),
    ('2026-05-25', 'Ascensión del Señor'),
    ('2026-06-15', 'Corpus Christi'),
    ('2026-06-22', 'Sagrado Corazón'),
    ('2026-06-29', 'San Pedro y San Pablo'),
    ('2026-07-20', 'Día de la Independencia'),
    ('2026-08-07', 'Batalla de Boyacá'),
    ('2026-08-17', 'Asunción de la Virgen'),
    ('2026-10-12', 'Día de la Raza'),
    ('2026-11-02', 'Día de Todos los Santos'),
    ('2026-11-16', 'Independencia de Cartagena'),
    ('2026-12-08', 'Inmaculada Concepción'),
    ('2026-12-25', 'Navidad');
GO

PRINT 'Festivos insertados.';

-- ============================================================
-- SEDES (8 sedes FODUN)
-- ============================================================
INSERT INTO dbo.Sedes (IdTipoSede, IdMunicipio, Nombre, NombreCorto, Descripcion, Direccion, CapacidadTotal) VALUES
    -- Sede 1
    (1, 1, 'Sede Recreativa Villeta', 'Villeta',
     'Sede recreativa ubicada en el barrio San Jorge, a poca distancia de la plaza central de Villeta '
     + 'en la Provincia del Gualivá, Cundinamarca. Distante de Bogotá 90 kilómetros, aproximadamente '
     + 'una hora y media por la autopista Bogotá-Medellín. Área aproximada de 1 fanegada con amplias zonas verdes. '
     + 'Servicios: sala de estar y televisión, sala de conferencias para 20 personas, zona de juegos: billar, '
     + 'tenis de mesa, futbolín y juegos de mesa. Área para comedores, 4 cocinetas equipadas, baños de emergencia, '
     + 'vestieres y lockers para visita día, tienda, sauna, jacuzzi, piscina, cancha de microfútbol y zonas verdes.',
     'Barrio San Jorge, Villeta, Cundinamarca', 32),

    -- Sede 2
    (1, 2, 'Sede Recreativa El Placer - Fusagasugá', 'El Placer',
     'Sede recreativa ubicada en la vereda El Placer del municipio de Fusagasugá, a unos 10 minutos '
     + 'del casco urbano. Capacidad total de alojamiento: hasta 34 personas.',
     'Vereda El Placer, Fusagasugá, Cundinamarca', 34),

    -- Sede 3
    (1, 3, 'Sede Recreativa Gonzalo Morante - Chinchiná', 'Gonzalo Morante',
     'Sede recreativa en Chinchiná, Caldas. Capacidad total para hospedaje: hasta 30 personas.',
     'Chinchiná, Caldas', 30),

    -- Sede 4
    (1, 4, 'Sede Recreativa Tablones - Palmira', 'Tablones',
     'Sede recreativa en Palmira, Valle del Cauca. Capacidad total para hospedaje: 24 personas.',
     'Palmira, Valle del Cauca', 24),

    -- Sede 5
    (1, 5, 'Sede Recreativa Manguruma - Santa Fe de Antioquia', 'Manguruma',
     'Sede recreativa en Santa Fe de Antioquia. Capacidad total: 46 personas.',
     'Santa Fe de Antioquia, Antioquia', 46),

    -- Sede 6
    (1, 6, 'Sede Recreativa Federman - Bogotá', 'Federman',
     'Sede recreativa en Bogotá. Servicios: zona húmeda (baño turco, sauna, jacuzzi), gimnasio, '
     + 'sala de masajes, billar, juegos de mesa, salas de música y video, lectura, cafetería y sala social. '
     + 'Ejercicios: bicicleta estática, aeróbicos, pilates y rumba tropical (Lunes a viernes 5:30-6:30 p.m.). '
     + 'Cuenta con 4 habitaciones para alojamiento de los asociados.',
     'Bogotá D.C.', 8),

    -- Sede 7
    (2, 7, 'Edificio Suramericana - Medellín', 'Suramericana',
     'Ubicado en la Calle 49B N° 64B-15 en el Edificio Suramericana N° 6 Apartamento 1204. '
     + 'Cerca del campus de la Universidad Nacional de Colombia. Habitaciones disponibles para asociados.',
     'Calle 49B N° 64B-15, Medellín, Antioquia', 10),

    -- Sede 8
    (2, 8, 'Edificio Reina 1 - Santa Marta', 'Reina 1',
     'Ubicado en el Edificio REINA 1 de la Carrera 3 N° 7-85, centro urbano y turístico El Rodadero, '
     + 'a tres cuadras de la playa. Tres apartamentos disponibles para asociados.',
     'Carrera 3 N° 7-85, El Rodadero, Santa Marta, Magdalena', 20);
GO

PRINT 'Sedes insertadas.';

-- ============================================================
-- ALOJAMIENTOS
-- Se insertan en orden por sede.
-- NumeroHabitacionesTarifa: 1=tarifa $70K, 2=tarifa $90K, NULL=tarifa propia
-- ============================================================

-- -------------------------------------------------------
-- SEDE 1: VILLETA (IdSede=1)
-- 8 habitaciones idénticas: cama doble + camarote, baño, nevera, TV, terraza cubierta
-- Cap: 4 personas c/u | NumHabTarifa: 1
-- -------------------------------------------------------
INSERT INTO dbo.Alojamientos (IdSede, IdTipoAlojamiento, Numero, Nombre, Descripcion,
    NumeroHabitaciones, CapacidadMaxima, NumeroHabitacionesTarifa)
SELECT
    1,   -- Villeta
    1,   -- Habitación 1 Alcoba
    CAST(n AS NVARCHAR(10)),
    'Habitación ' + CAST(n AS NVARCHAR(10)),
    'Habitación con una alcoba que tiene cama doble y camarote, baño privado, nevera, televisor y terraza cubierta.',
    1, 4, 1
FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8)) AS nums(n);
GO
-- Alojamientos 1-8

-- -------------------------------------------------------
-- SEDE 2: EL PLACER - FUSAGASUGÁ (IdSede=2)
-- -------------------------------------------------------
-- Alojamiento 1: 2 hab, baño, TV. Hab1: cama doble+sencilla, Hab2: sencilla. Cap~4
INSERT INTO dbo.Alojamientos (IdSede, IdTipoAlojamiento, Numero, Nombre, Descripcion,
    NumeroHabitaciones, CapacidadMaxima, NumeroHabitacionesTarifa) VALUES
(2, 3, '1', 'Alojamiento 1',
 'Dos habitaciones, baño y televisor. Habitación 1: cama doble y una sencilla. Habitación 2: una cama sencilla.',
 2, 4, 2),
-- Alojamiento 2: 2 hab, baño, TV. Hab1: cama doble, Hab2: 4 sencillas. Cap~6
(2, 3, '2', 'Alojamiento 2',
 'Dos habitaciones, baño y televisor. Habitación 1: cama doble. Habitación 2: cuatro camas sencillas.',
 2, 6, 2),
-- Alojamiento 3: 1 hab, cama doble+2 sencillas, baño, TV. Cap~4
(2, 2, '3', 'Alojamiento 3',
 'Una habitación con cama doble y dos camas sencillas, baño y televisor.',
 1, 4, 1),
-- Alojamiento 4: 2 hab, igual al 1. Cap~4
(2, 3, '4', 'Alojamiento 4',
 'Dos habitaciones, baño y televisor. Habitación 1: cama doble y una sencilla. Habitación 2: una cama sencilla.',
 2, 4, 2),
-- Cabañas 5-8: sala+sofá cama+TV, baño, hab cama doble+sencilla, cocineta, nevera, terraza
-- "Alojamientos nuevos" → tarifa 2 habitaciones (NumHabTarifa=2)
(2, 4, '5', 'Cabaña 5',
 'Bloque de cabañas. Sala de estar con sofá cama y televisor, baño, habitación con cama doble y cama sencilla, cocineta equipada, nevera y terraza comedor.',
 1, 4, 2),
(2, 4, '6', 'Cabaña 6',
 'Bloque de cabañas. Sala de estar con sofá cama y televisor, baño, habitación con cama doble y cama sencilla, cocineta equipada, nevera y terraza comedor.',
 1, 4, 2),
(2, 4, '7', 'Cabaña 7',
 'Bloque de cabañas. Sala de estar con sofá cama y televisor, baño, habitación con cama doble y cama sencilla, cocineta equipada, nevera y terraza comedor.',
 1, 4, 2),
(2, 4, '8', 'Cabaña 8',
 'Bloque de cabañas. Sala de estar con sofá cama y televisor, baño, habitación con cama doble y cama sencilla, cocineta equipada, nevera y terraza comedor.',
 1, 4, 2);
GO
-- Alojamientos 9-16

-- -------------------------------------------------------
-- SEDE 3: GONZALO MORANTE - CHINCHINÁ (IdSede=3)
-- -------------------------------------------------------
INSERT INTO dbo.Alojamientos (IdSede, IdTipoAlojamiento, Numero, Nombre, Descripcion,
    NumeroHabitaciones, CapacidadMaxima, NumeroHabitacionesTarifa) VALUES
(3, 3, '1', 'Alojamiento 1',
 'Cocineta, baño, televisor y 2 habitaciones. Hab 1: dos camas sencillas más dos adicionales. Hab 2: cama doble y una sencilla.',
 2, 6, 2),
(3, 3, '2', 'Alojamiento 2',
 'Cocineta, baño, televisor y 2 habitaciones. Hab 1: cama doble más una auxiliar doble. Hab 2: dos camas sencillas más dos auxiliares.',
 2, 6, 2),
(3, 5, '3', 'Cabaña Tipo A - Alojamiento 3',
 'Cabaña Tipo A. Cocineta, dos baños, sala comedor, televisor y dos habitaciones. Hab 1: cama doble. Hab 2: dos camas sencillas más dos auxiliares.',
 2, 6, 2),
(3, 2, '4', 'Alojamiento 4',
 'Cocineta, baño, televisor y una habitación con cama doble y una cama sencilla.',
 1, 3, 1),
(3, 4, '5', 'Cabaña Tipo B - Alojamiento 5',
 'Cabaña Tipo B. Cocineta, baño, sala con sofá, televisor, una habitación con cama doble y una cama sencilla.',
 1, 3, 1),
(3, 4, '6', 'Cabaña Tipo B - Alojamiento 6',
 'Cabaña Tipo B. Cocineta, baño, sala con sofá, televisor, una habitación con cama doble y una cama sencilla.',
 1, 3, 1);
GO
-- Alojamientos 17-22

-- -------------------------------------------------------
-- SEDE 4: TABLONES - PALMIRA (IdSede=4)
-- -------------------------------------------------------
INSERT INTO dbo.Alojamientos (IdSede, IdTipoAlojamiento, Numero, Nombre, Descripcion,
    NumeroHabitaciones, CapacidadMaxima, NumeroHabitacionesTarifa) VALUES
(4, 2, '1', 'Alojamiento 1',
 'Una habitación con cama doble y un camarote. Televisor, baño, cocineta con nevera y comedor.',
 1, 4, 1),
(4, 2, '2', 'Alojamiento 2',
 'Una habitación con cama doble y un camarote. Televisor, baño y cocineta con nevera, comedor.',
 1, 4, 1),
(4, 3, '3', 'Alojamiento 3',
 'Dos habitaciones. Hab 1: cama doble y un camarote. Hab 2: dos camarotes. Sala de estar con televisor, baño y cocineta.',
 2, 8, 2),
(4, 3, '4', 'Alojamiento 4',
 'Dos habitaciones. Hab 1: cama doble y un camarote. Hab 2: dos camarotes. Sala de estar con televisor, baño y cocineta.',
 2, 8, 2);
GO
-- Alojamientos 23-26

-- -------------------------------------------------------
-- SEDE 5: MANGURUMA - SANTA FE DE ANTIOQUIA (IdSede=5)
-- -------------------------------------------------------
INSERT INTO dbo.Alojamientos (IdSede, IdTipoAlojamiento, Numero, Nombre, Descripcion,
    NumeroHabitaciones, CapacidadMaxima, NumeroHabitacionesTarifa) VALUES
-- Alojamientos antiguos
(5, 1, '1', 'Alojamiento 1',
 'Una cama doble y un camarote. Baño y terraza. Televisor.',
 1, 4, 1),
(5, 1, '2', 'Alojamiento 2',
 'Una cama doble, un camarote y un sofá-cama. Baño y terraza. Televisor.',
 1, 5, 1),
(5, 1, '3', 'Alojamiento 3',
 'Una cama doble, un camarote y un sofá-cama. Baño y terraza. Televisor.',
 1, 5, 1),
-- Bloque Nuevo (8 alojamientos) → tarifa 2 habitaciones
(5, 4, '4', 'Bloque Nuevo - Alojamiento 4',
 'Bloque nuevo. Habitación con dos camas gemelas y un camarote, baño, terraza-comedor y cocina. Nevera y televisor.',
 1, 4, 2),
(5, 4, '5', 'Bloque Nuevo - Alojamiento 5',
 'Bloque nuevo. Habitación con dos camas gemelas y un camarote, baño, terraza-comedor y cocina. Nevera y televisor.',
 1, 4, 2),
(5, 4, '6', 'Bloque Nuevo - Alojamiento 6',
 'Bloque nuevo. Habitación con dos camas gemelas y un camarote, baño, terraza-comedor y cocina. Nevera y televisor.',
 1, 4, 2),
(5, 4, '7', 'Bloque Nuevo - Alojamiento 7',
 'Bloque nuevo. Habitación con dos camas gemelas y un camarote, baño, terraza-comedor y cocina. Nevera y televisor.',
 1, 4, 2),
(5, 4, '8', 'Bloque Nuevo - Alojamiento 8',
 'Bloque nuevo. Habitación con dos camas gemelas y un camarote, baño, terraza-comedor y cocina. Nevera y televisor.',
 1, 4, 2),
(5, 4, '9', 'Bloque Nuevo - Alojamiento 9',
 'Bloque nuevo. Habitación con dos camas gemelas y un camarote, baño, terraza-comedor y cocina. Nevera y televisor.',
 1, 4, 2),
(5, 4, '10', 'Bloque Nuevo - Alojamiento 10',
 'Bloque nuevo. Habitación con dos camas gemelas y un camarote, baño, terraza-comedor y cocina. Nevera y televisor.',
 1, 4, 2),
(5, 4, '11', 'Bloque Nuevo - Alojamiento 11',
 'Bloque nuevo. Habitación con dos camas gemelas y un camarote, baño, terraza-comedor y cocina. Nevera y televisor.',
 1, 4, 2);
GO
-- Alojamientos 27-37

-- -------------------------------------------------------
-- SEDE 6: FEDERMAN - BOGOTÁ (IdSede=6)
-- -------------------------------------------------------
INSERT INTO dbo.Alojamientos (IdSede, IdTipoAlojamiento, Numero, Nombre, Descripcion,
    NumeroHabitaciones, CapacidadMaxima, NumeroHabitacionesTarifa) VALUES
(6, 1, '1', 'Habitación 1', 'Habitación para alojamiento de asociados en Sede Federman Bogotá.', 1, 2, 1),
(6, 1, '2', 'Habitación 2', 'Habitación para alojamiento de asociados en Sede Federman Bogotá.', 1, 2, 1),
(6, 1, '3', 'Habitación 3', 'Habitación para alojamiento de asociados en Sede Federman Bogotá.', 1, 2, 1),
(6, 1, '4', 'Habitación 4', 'Habitación para alojamiento de asociados en Sede Federman Bogotá.', 1, 2, 1);
GO
-- Alojamientos 38-41

-- -------------------------------------------------------
-- SEDE 7: EDIFICIO SURAMERICANA - MEDELLÍN (IdSede=7)
-- 5 habitaciones; tarifa por habitación según núm. personas
-- -------------------------------------------------------
INSERT INTO dbo.Alojamientos (IdSede, IdTipoAlojamiento, Numero, Nombre, Descripcion,
    NumeroHabitaciones, CapacidadMaxima, NumeroHabitacionesTarifa) VALUES
(7, 7, '1', 'Habitación 1',
 'Habitación 1 con dos camas sencillas y baño privado.',
 1, 2, NULL),
(7, 7, '2', 'Habitación 2',
 'Habitación 2 con dos camas sencillas.',
 1, 2, NULL),
(7, 7, '3', 'Habitación 3',
 'Habitación 3 con dos camas sencillas.',
 1, 2, NULL),
(7, 7, '4', 'Habitación 4',
 'Habitación 4 con dos camas sencillas.',
 1, 2, NULL),
(7, 7, '5', 'Habitación 5',
 'Habitación 5 con una cama sencilla y baño privado.',
 1, 2, NULL);
GO
-- Alojamientos 42-46

-- -------------------------------------------------------
-- SEDE 8: EDIFICIO REINA 1 - SANTA MARTA (IdSede=8)
-- 3 apartamentos con tarifa propia según temporada
-- -------------------------------------------------------
INSERT INTO dbo.Alojamientos (IdSede, IdTipoAlojamiento, Numero, Nombre, Descripcion,
    NumeroHabitaciones, CapacidadMaxima, NumeroHabitacionesTarifa) VALUES
(8, 6, '202', 'Apartamento 202',
 'Apartamento 202. Sala comedor, cocina, 2 baños, tres habitaciones y un sitio para parqueo. '
 + 'Capacidad máxima: 8 personas.',
 3, 8, NULL),
(8, 6, '301', 'Apartamento 301',
 'Apartamento 301. Sala comedor, cocina, 1 baño, dos habitaciones y un sitio para parqueo. '
 + 'Capacidad máxima: 6 personas.',
 2, 6, NULL),
(8, 6, '401', 'Apartamento 401',
 'Apartamento 401. Sala comedor, cocina, 1 baño, dos habitaciones y un sitio para parqueo. '
 + 'Capacidad máxima: 6 personas.',
 2, 6, NULL);
GO
-- Alojamientos 47-49

PRINT 'Alojamientos insertados.';

-- ============================================================
-- TARIFAS
-- Lógica de resolución en SPs:
--   1. IdAlojamiento IS NOT NULL → tarifa específica de apartamento
--   2. IdSede IS NOT NULL         → tarifa específica de sede
--   3. IdSede IS NULL             → tarifa genérica sedes recreativas
-- ============================================================

-- -------------------------------------------------------
-- TARIFAS GENÉRICAS: SEDES RECREATIVAS
-- Aplica a: Villeta, El Placer, Gonzalo Morante, Tablones, Manguruma
-- (IdSede=NULL indica "todas las sedes recreativas estándar")
-- -------------------------------------------------------

-- Temporada Ordinaria (IdTemporada=1) - Tarifa base
INSERT INTO dbo.Tarifas (IdSede, IdAlojamiento, IdTemporada, NumeroHabitacionesTarifa,
    PersonasIncluidas, TarifaBase, ValorPersonaAdicional, EsTarifaEspecial, Descripcion) VALUES
(NULL, NULL, 1, 1, 4, 70000, 16000, 0,
 'Sedes recreativas - 1 hab/noche - hasta 4 personas - tarifa ordinaria'),
(NULL, NULL, 1, 2, 4, 90000, 16000, 0,
 'Sedes recreativas - 2 hab/noche - hasta 4 personas - tarifa ordinaria');

-- Temporada Especial Semana (IdTemporada=3) - Promo Lun-Jue
INSERT INTO dbo.Tarifas (IdSede, IdAlojamiento, IdTemporada, NumeroHabitacionesTarifa,
    PersonasIncluidas, TarifaBase, ValorPersonaAdicional, EsTarifaEspecial, Descripcion) VALUES
(NULL, NULL, 3, 1, 4, 27000, 11000, 1,
 'Sedes recreativas - 1 hab/noche - hasta 4 personas - tarifa especial Lun-Jue'),
(NULL, NULL, 3, 2, 4, 37000, 11000, 1,
 'Sedes recreativas - 2 hab/noche - hasta 4 personas - tarifa especial Lun-Jue');

-- Alta temporada (IdTemporada=2) para sedes recreativas
-- Usa misma tarifa ordinaria, sin promo especial
INSERT INTO dbo.Tarifas (IdSede, IdAlojamiento, IdTemporada, NumeroHabitacionesTarifa,
    PersonasIncluidas, TarifaBase, ValorPersonaAdicional, EsTarifaEspecial, Descripcion) VALUES
(NULL, NULL, 2, 1, 4, 70000, 16000, 0,
 'Sedes recreativas - 1 hab/noche - hasta 4 personas - temporada alta'),
(NULL, NULL, 2, 2, 4, 90000, 16000, 0,
 'Sedes recreativas - 2 hab/noche - hasta 4 personas - temporada alta');
GO

-- -------------------------------------------------------
-- TARIFAS EDIFICIO SURAMERICANA - MEDELLÍN (IdSede=7)
-- Por habitación/noche según número de personas (1 o 2)
-- -------------------------------------------------------
INSERT INTO dbo.Tarifas (IdSede, IdAlojamiento, IdTemporada, NumeroHabitacionesTarifa,
    PersonasIncluidas, TarifaBase, ValorPersonaAdicional, EsTarifaEspecial, Descripcion) VALUES
(7, NULL, 1, NULL, 1, 63000, 12000, 0,
 'Suramericana Medellín - habitación/noche - 1 persona'),
(7, NULL, 1, NULL, 2, 75000, 0, 0,
 'Suramericana Medellín - habitación/noche - 2 personas');
GO

-- -------------------------------------------------------
-- TARIFAS EDIFICIO REINA 1 - SANTA MARTA (por apartamento)
-- Los IdAlojamiento de los 3 apartamentos son 47, 48, 49
-- Apartamento 202=47, Apartamento 301=48, Apartamento 401=49
-- -------------------------------------------------------

-- Apartamento 202 (IdAlojamiento=47) - Baja temporada
INSERT INTO dbo.Tarifas (IdSede, IdAlojamiento, IdTemporada, NumeroHabitacionesTarifa,
    PersonasIncluidas, TarifaBase, ValorPersonaAdicional, EsTarifaEspecial, Descripcion) VALUES
(8, 47, 4, NULL, 8, 103000, 0, 0,
 'Apto 202 Reina 1 Santa Marta - hasta 8 personas - baja temporada'),
(8, 47, 2, NULL, 8, 143000, 0, 0,
 'Apto 202 Reina 1 Santa Marta - hasta 8 personas - alta temporada'),

-- Apartamento 301 (IdAlojamiento=48) - Baja y Alta
(8, 48, 4, NULL, 6, 89000, 0, 0,
 'Apto 301 Reina 1 Santa Marta - hasta 6 personas - baja temporada'),
(8, 48, 2, NULL, 6, 124000, 0, 0,
 'Apto 301 Reina 1 Santa Marta - hasta 6 personas - alta temporada'),

-- Apartamento 401 (IdAlojamiento=49) - Baja y Alta
(8, 49, 4, NULL, 6, 89000, 0, 0,
 'Apto 401 Reina 1 Santa Marta - hasta 6 personas - baja temporada'),
(8, 49, 2, NULL, 6, 124000, 0, 0,
 'Apto 401 Reina 1 Santa Marta - hasta 6 personas - alta temporada');
GO

PRINT 'Tarifas insertadas.';

-- ============================================================
-- VERIFICACIÓN RÁPIDA
-- ============================================================
SELECT 'Departamentos' AS Tabla, COUNT(*) AS Total FROM dbo.Departamentos
UNION ALL SELECT 'Municipios', COUNT(*) FROM dbo.Municipios
UNION ALL SELECT 'TiposSede', COUNT(*) FROM dbo.TiposSede
UNION ALL SELECT 'TiposAlojamiento', COUNT(*) FROM dbo.TiposAlojamiento
UNION ALL SELECT 'Sedes', COUNT(*) FROM dbo.Sedes
UNION ALL SELECT 'Alojamientos', COUNT(*) FROM dbo.Alojamientos
UNION ALL SELECT 'Temporadas', COUNT(*) FROM dbo.Temporadas
UNION ALL SELECT 'Tarifas', COUNT(*) FROM dbo.Tarifas
UNION ALL SELECT 'EstadosReserva', COUNT(*) FROM dbo.EstadosReserva
UNION ALL SELECT 'ServiciosAdicionales', COUNT(*) FROM dbo.ServiciosAdicionales
UNION ALL SELECT 'Festivos', COUNT(*) FROM dbo.Festivos;
GO

PRINT '=== Datos iniciales cargados correctamente. ===';
GO
