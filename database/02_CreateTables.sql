-- ============================================================
-- DB_ReservasFodun
-- Sistema de Reservas - Sedes Recreativas y Apartamentos FODUN
-- ============================================================
-- Script  : 02_CreateTables.sql
-- Autor   : Arquitecto BD Senior
-- Fecha   : 2026
-- Desc    : Creación de todas las tablas del sistema
-- ============================================================
-- ORDEN DE EJECUCIÓN (respeta dependencias FK):
--  1. Departamentos
--  2. Municipios
--  3. TiposSede
--  4. TiposAlojamiento
--  5. Caracteristicas
--  6. Temporadas
--  7. EstadosReserva
--  8. ServiciosAdicionales
--  9. Festivos
-- 10. Sedes
-- 11. Alojamientos
-- 12. AlojamientoCaracteristicas
-- 13. Tarifas
-- 14. PerfilUsuario  (referencia a AspNetUsers.Id post-migration)
-- 15. Reservas       (referencia a AspNetUsers.Id post-migration)
-- 16. ReservaDetalles
-- 17. ReservaServiciosAdicionales
-- 18. Pagos
-- ============================================================

USE DB_ReservasFodun;
GO

-- ============================================================
-- 1. DEPARTAMENTOS
--    Catálogo de departamentos de Colombia
-- ============================================================
IF OBJECT_ID('dbo.Departamentos', 'U') IS NOT NULL
    DROP TABLE dbo.Departamentos;
GO

CREATE TABLE dbo.Departamentos (
    IdDepartamento      INT             IDENTITY(1,1)   NOT NULL,
    Nombre              NVARCHAR(100)                   NOT NULL,
    CodigoDane          NVARCHAR(5)                     NULL,
    Activo              BIT             NOT NULL        DEFAULT 1,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),
    FechaModificacion   DATETIME2                       NULL,
    UsuarioCreacion     NVARCHAR(256)                   NULL,
    UsuarioModificacion NVARCHAR(256)                   NULL,

    CONSTRAINT PK_Departamentos PRIMARY KEY (IdDepartamento),
    CONSTRAINT UQ_Departamentos_Nombre UNIQUE (Nombre)
);
GO

PRINT 'Tabla Departamentos creada.';

-- ============================================================
-- 2. MUNICIPIOS
--    Catálogo de municipios vinculados a departamento
-- ============================================================
IF OBJECT_ID('dbo.Municipios', 'U') IS NOT NULL
    DROP TABLE dbo.Municipios;
GO

CREATE TABLE dbo.Municipios (
    IdMunicipio         INT             IDENTITY(1,1)   NOT NULL,
    IdDepartamento      INT                             NOT NULL,
    Nombre              NVARCHAR(150)                   NOT NULL,
    CodigoDane          NVARCHAR(8)                     NULL,
    Activo              BIT             NOT NULL        DEFAULT 1,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),
    FechaModificacion   DATETIME2                       NULL,
    UsuarioCreacion     NVARCHAR(256)                   NULL,
    UsuarioModificacion NVARCHAR(256)                   NULL,

    CONSTRAINT PK_Municipios PRIMARY KEY (IdMunicipio),
    CONSTRAINT FK_Municipios_Departamentos
        FOREIGN KEY (IdDepartamento) REFERENCES dbo.Departamentos(IdDepartamento)
);
GO

PRINT 'Tabla Municipios creada.';

-- ============================================================
-- 3. TIPOS DE SEDE
--    Clasificación: Sede Recreativa / Apartamento
-- ============================================================
IF OBJECT_ID('dbo.TiposSede', 'U') IS NOT NULL
    DROP TABLE dbo.TiposSede;
GO

CREATE TABLE dbo.TiposSede (
    IdTipoSede          INT             IDENTITY(1,1)   NOT NULL,
    Nombre              NVARCHAR(100)                   NOT NULL,
    Descripcion         NVARCHAR(500)                   NULL,
    Activo              BIT             NOT NULL        DEFAULT 1,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),
    FechaModificacion   DATETIME2                       NULL,
    UsuarioCreacion     NVARCHAR(256)                   NULL,
    UsuarioModificacion NVARCHAR(256)                   NULL,

    CONSTRAINT PK_TiposSede PRIMARY KEY (IdTipoSede),
    CONSTRAINT UQ_TiposSede_Nombre UNIQUE (Nombre)
);
GO

PRINT 'Tabla TiposSede creada.';

-- ============================================================
-- 4. TIPOS DE ALOJAMIENTO
--    Clasificación: Habitación, Cabaña 1 hab, Cabaña 2 hab, Apartamento
-- ============================================================
IF OBJECT_ID('dbo.TiposAlojamiento', 'U') IS NOT NULL
    DROP TABLE dbo.TiposAlojamiento;
GO

CREATE TABLE dbo.TiposAlojamiento (
    IdTipoAlojamiento   INT             IDENTITY(1,1)   NOT NULL,
    Nombre              NVARCHAR(100)                   NOT NULL,
    Descripcion         NVARCHAR(500)                   NULL,
    Activo              BIT             NOT NULL        DEFAULT 1,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),
    FechaModificacion   DATETIME2                       NULL,
    UsuarioCreacion     NVARCHAR(256)                   NULL,
    UsuarioModificacion NVARCHAR(256)                   NULL,

    CONSTRAINT PK_TiposAlojamiento PRIMARY KEY (IdTipoAlojamiento),
    CONSTRAINT UQ_TiposAlojamiento_Nombre UNIQUE (Nombre)
);
GO

PRINT 'Tabla TiposAlojamiento creada.';

-- ============================================================
-- 5. CARACTERÍSTICAS
--    Amenidades: Cama doble, Camarote, Baño, Nevera, TV, etc.
-- ============================================================
IF OBJECT_ID('dbo.Caracteristicas', 'U') IS NOT NULL
    DROP TABLE dbo.Caracteristicas;
GO

CREATE TABLE dbo.Caracteristicas (
    IdCaracteristica    INT             IDENTITY(1,1)   NOT NULL,
    Nombre              NVARCHAR(100)                   NOT NULL,
    Icono               NVARCHAR(100)                   NULL,   -- CSS icon class
    Activo              BIT             NOT NULL        DEFAULT 1,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),

    CONSTRAINT PK_Caracteristicas PRIMARY KEY (IdCaracteristica),
    CONSTRAINT UQ_Caracteristicas_Nombre UNIQUE (Nombre)
);
GO

PRINT 'Tabla Caracteristicas creada.';

-- ============================================================
-- 6. TEMPORADAS
--    Ordinaria, Alta, Especial (Lun-Jue promo), Baja
-- ============================================================
IF OBJECT_ID('dbo.Temporadas', 'U') IS NOT NULL
    DROP TABLE dbo.Temporadas;
GO

CREATE TABLE dbo.Temporadas (
    IdTemporada         INT             IDENTITY(1,1)   NOT NULL,
    Nombre              NVARCHAR(100)                   NOT NULL,
    Descripcion         NVARCHAR(500)                   NULL,
    -- EsTemporadaAlta: Alta bloquea tarifa especial (Mon-Thu)
    EsTemporadaAlta     BIT             NOT NULL        DEFAULT 0,
    -- EsTarifaEspecial: Tarifa promocional Lun-Jue
    EsTarifaEspecial    BIT             NOT NULL        DEFAULT 0,
    -- DiasAplicacion: '1,2,3,4' = Dom,Lun,Mar,Mie (DATEPART dw con DATEFIRST=7)
    -- NULL = aplica todos los días
    DiasAplicacion      NVARCHAR(20)                    NULL,
    Activo              BIT             NOT NULL        DEFAULT 1,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),
    FechaModificacion   DATETIME2                       NULL,
    UsuarioCreacion     NVARCHAR(256)                   NULL,
    UsuarioModificacion NVARCHAR(256)                   NULL,

    CONSTRAINT PK_Temporadas PRIMARY KEY (IdTemporada),
    CONSTRAINT UQ_Temporadas_Nombre UNIQUE (Nombre)
);
GO

PRINT 'Tabla Temporadas creada.';

-- ============================================================
-- 7. ESTADOS DE RESERVA
--    Pendiente, Confirmada, Pagada, Cancelada
-- ============================================================
IF OBJECT_ID('dbo.EstadosReserva', 'U') IS NOT NULL
    DROP TABLE dbo.EstadosReserva;
GO

CREATE TABLE dbo.EstadosReserva (
    IdEstadoReserva     INT             IDENTITY(1,1)   NOT NULL,
    Nombre              NVARCHAR(50)                    NOT NULL,
    Descripcion         NVARCHAR(300)                   NULL,
    -- BloqueoDisponibilidad: Si TRUE, reservas en este estado bloquean el alojamiento
    BloqueoDisponibilidad BIT           NOT NULL        DEFAULT 1,
    Activo              BIT             NOT NULL        DEFAULT 1,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),

    CONSTRAINT PK_EstadosReserva PRIMARY KEY (IdEstadoReserva),
    CONSTRAINT UQ_EstadosReserva_Nombre UNIQUE (Nombre)
);
GO

PRINT 'Tabla EstadosReserva creada.';

-- ============================================================
-- 8. SERVICIOS ADICIONALES
--    Lavandería, Visita Día Acompañante
-- ============================================================
IF OBJECT_ID('dbo.ServiciosAdicionales', 'U') IS NOT NULL
    DROP TABLE dbo.ServiciosAdicionales;
GO

CREATE TABLE dbo.ServiciosAdicionales (
    IdServicio          INT             IDENTITY(1,1)   NOT NULL,
    Nombre              NVARCHAR(100)                   NOT NULL,
    Descripcion         NVARCHAR(500)                   NULL,
    Valor               DECIMAL(18,2)   NOT NULL        DEFAULT 0,
    -- PorPersona: TRUE = se cobra por persona (ej: visita día)
    PorPersona          BIT             NOT NULL        DEFAULT 0,
    -- PorNoche: TRUE = se cobra por noche (ej: lavandería)
    PorNoche            BIT             NOT NULL        DEFAULT 0,
    Activo              BIT             NOT NULL        DEFAULT 1,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),
    FechaModificacion   DATETIME2                       NULL,
    UsuarioCreacion     NVARCHAR(256)                   NULL,
    UsuarioModificacion NVARCHAR(256)                   NULL,

    CONSTRAINT PK_ServiciosAdicionales PRIMARY KEY (IdServicio),
    CONSTRAINT UQ_ServiciosAdicionales_Nombre UNIQUE (Nombre),
    CONSTRAINT CK_ServiciosAdicionales_Valor CHECK (Valor >= 0)
);
GO

PRINT 'Tabla ServiciosAdicionales creada.';

-- ============================================================
-- 9. FESTIVOS
--    Calendario de días festivos (afecta tarifa especial)
-- ============================================================
IF OBJECT_ID('dbo.Festivos', 'U') IS NOT NULL
    DROP TABLE dbo.Festivos;
GO

CREATE TABLE dbo.Festivos (
    IdFestivo           INT             IDENTITY(1,1)   NOT NULL,
    Fecha               DATE                            NOT NULL,
    Nombre              NVARCHAR(100)                   NOT NULL,
    Activo              BIT             NOT NULL        DEFAULT 1,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),

    CONSTRAINT PK_Festivos PRIMARY KEY (IdFestivo),
    CONSTRAINT UQ_Festivos_Fecha UNIQUE (Fecha)
);
GO

PRINT 'Tabla Festivos creada.';

-- ============================================================
-- 10. SEDES
--     Sedes recreativas y edificios de apartamentos FODUN
-- ============================================================
IF OBJECT_ID('dbo.Sedes', 'U') IS NOT NULL
    DROP TABLE dbo.Sedes;
GO

CREATE TABLE dbo.Sedes (
    IdSede              INT             IDENTITY(1,1)   NOT NULL,
    IdTipoSede          INT                             NOT NULL,
    IdMunicipio         INT                             NOT NULL,
    Nombre              NVARCHAR(200)                   NOT NULL,
    NombreCorto         NVARCHAR(100)                   NULL,
    Descripcion         NVARCHAR(2000)                  NULL,
    Direccion           NVARCHAR(300)                   NULL,
    Telefono            NVARCHAR(50)                    NULL,
    Email               NVARCHAR(200)                   NULL,
    CapacidadTotal      INT             NOT NULL        DEFAULT 0,
    Latitud             DECIMAL(10,7)                   NULL,
    Longitud            DECIMAL(10,7)                   NULL,
    ImagenUrl           NVARCHAR(500)                   NULL,
    RutaLlegadaUrl      NVARCHAR(500)                   NULL,
    Activo              BIT             NOT NULL        DEFAULT 1,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),
    FechaModificacion   DATETIME2                       NULL,
    UsuarioCreacion     NVARCHAR(256)                   NULL,
    UsuarioModificacion NVARCHAR(256)                   NULL,

    CONSTRAINT PK_Sedes PRIMARY KEY (IdSede),
    CONSTRAINT FK_Sedes_TiposSede
        FOREIGN KEY (IdTipoSede) REFERENCES dbo.TiposSede(IdTipoSede),
    CONSTRAINT FK_Sedes_Municipios
        FOREIGN KEY (IdMunicipio) REFERENCES dbo.Municipios(IdMunicipio),
    CONSTRAINT CK_Sedes_CapacidadTotal CHECK (CapacidadTotal >= 0)
);
GO

PRINT 'Tabla Sedes creada.';

-- ============================================================
-- 11. ALOJAMIENTOS
--     Habitaciones, cabañas y apartamentos de cada sede
-- ============================================================
IF OBJECT_ID('dbo.Alojamientos', 'U') IS NOT NULL
    DROP TABLE dbo.Alojamientos;
GO

CREATE TABLE dbo.Alojamientos (
    IdAlojamiento           INT             IDENTITY(1,1)   NOT NULL,
    IdSede                  INT                             NOT NULL,
    IdTipoAlojamiento       INT                             NOT NULL,
    -- Numero: identificador interno (1, 2, 3, '202', '301', etc.)
    Numero                  NVARCHAR(10)                    NOT NULL,
    Nombre                  NVARCHAR(200)                   NOT NULL,
    Descripcion             NVARCHAR(2000)                  NULL,
    -- NumeroHabitaciones: cantidad física de habitaciones en el alojamiento
    NumeroHabitaciones      INT             NOT NULL        DEFAULT 1,
    -- CapacidadMaxima: número máximo de personas que puede alojar
    CapacidadMaxima         INT             NOT NULL        DEFAULT 1,
    -- NumeroHabitacionesTarifa: categoría tarifaria (1 o 2 para sedes recreativas)
    -- NULL para apartamentos con tarifa propia
    NumeroHabitacionesTarifa INT                            NULL,
    ImagenUrl               NVARCHAR(500)                   NULL,
    Activo                  BIT             NOT NULL        DEFAULT 1,
    FechaCreacion           DATETIME2       NOT NULL        DEFAULT GETDATE(),
    FechaModificacion       DATETIME2                       NULL,
    UsuarioCreacion         NVARCHAR(256)                   NULL,
    UsuarioModificacion     NVARCHAR(256)                   NULL,

    CONSTRAINT PK_Alojamientos PRIMARY KEY (IdAlojamiento),
    CONSTRAINT FK_Alojamientos_Sedes
        FOREIGN KEY (IdSede) REFERENCES dbo.Sedes(IdSede),
    CONSTRAINT FK_Alojamientos_TiposAlojamiento
        FOREIGN KEY (IdTipoAlojamiento) REFERENCES dbo.TiposAlojamiento(IdTipoAlojamiento),
    CONSTRAINT CK_Alojamientos_Capacidad CHECK (CapacidadMaxima > 0),
    CONSTRAINT CK_Alojamientos_NumHab CHECK (NumeroHabitaciones > 0),
    CONSTRAINT CK_Alojamientos_NumHabTarifa CHECK (NumeroHabitacionesTarifa IS NULL OR NumeroHabitacionesTarifa IN (1, 2)),
    CONSTRAINT UQ_Alojamientos_SedeNumero UNIQUE (IdSede, Numero)
);
GO

PRINT 'Tabla Alojamientos creada.';

-- ============================================================
-- 12. ALOJAMIENTO CARACTERÍSTICAS (relación N:M)
--     Amenidades de cada alojamiento
-- ============================================================
IF OBJECT_ID('dbo.AlojamientoCaracteristicas', 'U') IS NOT NULL
    DROP TABLE dbo.AlojamientoCaracteristicas;
GO

CREATE TABLE dbo.AlojamientoCaracteristicas (
    IdAlojamiento       INT                             NOT NULL,
    IdCaracteristica    INT                             NOT NULL,
    Cantidad            INT             NOT NULL        DEFAULT 1,
    Observacion         NVARCHAR(200)                   NULL,

    CONSTRAINT PK_AlojamientoCaracteristicas
        PRIMARY KEY (IdAlojamiento, IdCaracteristica),
    CONSTRAINT FK_AloCarac_Alojamiento
        FOREIGN KEY (IdAlojamiento) REFERENCES dbo.Alojamientos(IdAlojamiento),
    CONSTRAINT FK_AloCarac_Caracteristica
        FOREIGN KEY (IdCaracteristica) REFERENCES dbo.Caracteristicas(IdCaracteristica)
);
GO

PRINT 'Tabla AlojamientoCaracteristicas creada.';

-- ============================================================
-- 13. TARIFAS
--     Estructura flexible para múltiples reglas tarifarias:
--     - Por sede + NumHabTarifa (sedes recreativas genéricas)
--     - Por sede específica (Suramericana con 1 o 2 personas)
--     - Por alojamiento específico + temporada (Santa Marta)
-- ============================================================
IF OBJECT_ID('dbo.Tarifas', 'U') IS NOT NULL
    DROP TABLE dbo.Tarifas;
GO

CREATE TABLE dbo.Tarifas (
    IdTarifa                INT             IDENTITY(1,1)   NOT NULL,
    -- IdSede NULL = aplica a todas las sedes recreativas estándar
    IdSede                  INT                             NULL,
    -- IdAlojamiento NULL = aplica por sede/tipo
    IdAlojamiento           INT                             NULL,
    IdTemporada             INT                             NOT NULL,
    -- NumeroHabitacionesTarifa: 1 o 2 para sedes recreativas (NULL para apts)
    NumeroHabitacionesTarifa INT                            NULL,
    -- PersonasIncluidas: personas cubiertas por la TarifaBase
    PersonasIncluidas       INT             NOT NULL        DEFAULT 4,
    TarifaBase              DECIMAL(18,2)   NOT NULL,
    ValorPersonaAdicional   DECIMAL(18,2)   NOT NULL        DEFAULT 0,
    -- EsTarifaEspecial: TRUE = tarifa promocional Lun-Jue
    EsTarifaEspecial        BIT             NOT NULL        DEFAULT 0,
    Descripcion             NVARCHAR(300)                   NULL,
    Activo                  BIT             NOT NULL        DEFAULT 1,
    FechaCreacion           DATETIME2       NOT NULL        DEFAULT GETDATE(),
    FechaModificacion       DATETIME2                       NULL,
    UsuarioCreacion         NVARCHAR(256)                   NULL,
    UsuarioModificacion     NVARCHAR(256)                   NULL,

    CONSTRAINT PK_Tarifas PRIMARY KEY (IdTarifa),
    CONSTRAINT FK_Tarifas_Sedes
        FOREIGN KEY (IdSede) REFERENCES dbo.Sedes(IdSede),
    CONSTRAINT FK_Tarifas_Alojamientos
        FOREIGN KEY (IdAlojamiento) REFERENCES dbo.Alojamientos(IdAlojamiento),
    CONSTRAINT FK_Tarifas_Temporadas
        FOREIGN KEY (IdTemporada) REFERENCES dbo.Temporadas(IdTemporada),
    CONSTRAINT CK_Tarifas_Base CHECK (TarifaBase >= 0),
    CONSTRAINT CK_Tarifas_PersonasAdicionales CHECK (ValorPersonaAdicional >= 0),
    CONSTRAINT CK_Tarifas_PersonasIncluidas CHECK (PersonasIncluidas > 0)
);
GO

PRINT 'Tabla Tarifas creada.';

-- ============================================================
-- 14. PERFIL USUARIO
--     Extiende AspNetUsers con campos específicos de FODUN.
--     Referencia a AspNetUsers.Id (NVARCHAR 450).
--     NOTA: La tabla AspNetUsers se crea con EF Core Identity.
--     Ejecutar migrations ANTES de agregar la FK.
-- ============================================================
IF OBJECT_ID('dbo.PerfilUsuario', 'U') IS NOT NULL
    DROP TABLE dbo.PerfilUsuario;
GO

CREATE TABLE dbo.PerfilUsuario (
    IdPerfil            INT             IDENTITY(1,1)   NOT NULL,
    -- UserId referencia AspNetUsers.Id (creado por Identity)
    UserId              NVARCHAR(450)                   NOT NULL,
    NroDocumento        NVARCHAR(20)                    NOT NULL,
    Nombres             NVARCHAR(150)                   NOT NULL,
    Apellidos           NVARCHAR(150)                   NOT NULL,
    FechaNacimiento     DATE                            NULL,
    Celular             NVARCHAR(20)                    NULL,
    IdMunicipio         INT                             NULL,
    Barrio              NVARCHAR(150)                   NULL,
    DireccionResidencia NVARCHAR(300)                   NULL,
    TelefonoResidencia  NVARCHAR(20)                    NULL,
    AutorizaCorreo      BIT             NOT NULL        DEFAULT 1,
    AutorizaCelular     BIT             NOT NULL        DEFAULT 1,
    -- Pregunta/respuesta secreta para recuperación de contraseña
    IdPreguntaSecreta   INT                             NULL,
    RespuestaSecreta    NVARCHAR(200)                   NULL,
    Activo              BIT             NOT NULL        DEFAULT 1,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),
    FechaModificacion   DATETIME2                       NULL,

    CONSTRAINT PK_PerfilUsuario PRIMARY KEY (IdPerfil),
    CONSTRAINT UQ_PerfilUsuario_UserId UNIQUE (UserId),
    CONSTRAINT UQ_PerfilUsuario_NroDocumento UNIQUE (NroDocumento),
    CONSTRAINT FK_PerfilUsuario_Municipios
        FOREIGN KEY (IdMunicipio) REFERENCES dbo.Municipios(IdMunicipio)
    -- FK con AspNetUsers se agrega post-migration en 05_Indexes_Constraints.sql
);
GO

PRINT 'Tabla PerfilUsuario creada.';

-- ============================================================
-- 15. PREGUNTAS SECRETAS
--     Catálogo de preguntas para recuperación de contraseña
-- ============================================================
IF OBJECT_ID('dbo.PreguntasSecretas', 'U') IS NOT NULL
    DROP TABLE dbo.PreguntasSecretas;
GO

CREATE TABLE dbo.PreguntasSecretas (
    IdPreguntaSecreta   INT             IDENTITY(1,1)   NOT NULL,
    Pregunta            NVARCHAR(300)                   NOT NULL,
    Activo              BIT             NOT NULL        DEFAULT 1,

    CONSTRAINT PK_PreguntasSecretas PRIMARY KEY (IdPreguntaSecreta)
);
GO

ALTER TABLE dbo.PerfilUsuario
    ADD CONSTRAINT FK_PerfilUsuario_PreguntasSecretas
        FOREIGN KEY (IdPreguntaSecreta) REFERENCES dbo.PreguntasSecretas(IdPreguntaSecreta);
GO

PRINT 'Tabla PreguntasSecretas creada y FK agregada a PerfilUsuario.';

-- ============================================================
-- 16. RESERVAS
--     Cabecera de reserva vinculada al usuario autenticado
-- ============================================================
IF OBJECT_ID('dbo.Reservas', 'U') IS NOT NULL
    DROP TABLE dbo.Reservas;
GO

CREATE TABLE dbo.Reservas (
    IdReserva           INT             IDENTITY(1,1)   NOT NULL,
    -- UserId: ID del usuario autenticado (AspNetUsers.Id)
    UserId              NVARCHAR(450)                   NOT NULL,
    IdSede              INT                             NOT NULL,
    IdEstadoReserva     INT                             NOT NULL,
    -- Código visible para el usuario (ej: RES-2026-000001)
    CodigoReserva       NVARCHAR(30)                    NOT NULL,
    FechaReserva        DATETIME2       NOT NULL        DEFAULT GETDATE(),
    FechaLlegada        DATE                            NOT NULL,
    FechaSalida         DATE                            NOT NULL,
    NumeroNoches        INT             NOT NULL,
    NumeroPersonas      INT             NOT NULL,
    NumeroHabitaciones  INT             NOT NULL        DEFAULT 1,
    -- Desglose de valor
    ValorTarifaOrdinaria    DECIMAL(18,2) NOT NULL      DEFAULT 0,
    ValorTarifaEspecial     DECIMAL(18,2) NOT NULL      DEFAULT 0,
    ValorPersonasAdicionales DECIMAL(18,2) NOT NULL     DEFAULT 0,
    ValorLavanderia         DECIMAL(18,2) NOT NULL      DEFAULT 0,
    ValorOtrosServicios     DECIMAL(18,2) NOT NULL      DEFAULT 0,
    ValorSubtotal           DECIMAL(18,2) NOT NULL      DEFAULT 0,
    ValorTotal              DECIMAL(18,2) NOT NULL      DEFAULT 0,
    IncluyeLavanderia       BIT           NOT NULL      DEFAULT 0,
    DiasOrdinarios          INT           NOT NULL      DEFAULT 0,
    DiasEspeciales          INT           NOT NULL      DEFAULT 0,
    Observaciones           NVARCHAR(1000)              NULL,
    Activo              BIT             NOT NULL        DEFAULT 1,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),
    FechaModificacion   DATETIME2                       NULL,
    UsuarioCreacion     NVARCHAR(256)                   NULL,
    UsuarioModificacion NVARCHAR(256)                   NULL,

    CONSTRAINT PK_Reservas PRIMARY KEY (IdReserva),
    CONSTRAINT UQ_Reservas_Codigo UNIQUE (CodigoReserva),
    CONSTRAINT FK_Reservas_Sedes
        FOREIGN KEY (IdSede) REFERENCES dbo.Sedes(IdSede),
    CONSTRAINT FK_Reservas_EstadosReserva
        FOREIGN KEY (IdEstadoReserva) REFERENCES dbo.EstadosReserva(IdEstadoReserva),
    CONSTRAINT CK_Reservas_Fechas
        CHECK (FechaSalida > FechaLlegada),
    CONSTRAINT CK_Reservas_NumNoches CHECK (NumeroNoches > 0),
    CONSTRAINT CK_Reservas_NumPersonas CHECK (NumeroPersonas > 0),
    CONSTRAINT CK_Reservas_NumHabitaciones CHECK (NumeroHabitaciones > 0),
    CONSTRAINT CK_Reservas_ValorTotal CHECK (ValorTotal >= 0)
    -- FK con AspNetUsers se agrega post-migration en 05_Indexes_Constraints.sql
);
GO

PRINT 'Tabla Reservas creada.';

-- ============================================================
-- 17. RESERVA DETALLES
--     Alojamientos específicos incluidos en cada reserva
-- ============================================================
IF OBJECT_ID('dbo.ReservaDetalles', 'U') IS NOT NULL
    DROP TABLE dbo.ReservaDetalles;
GO

CREATE TABLE dbo.ReservaDetalles (
    IdReservaDetalle    INT             IDENTITY(1,1)   NOT NULL,
    IdReserva           INT                             NOT NULL,
    IdAlojamiento       INT                             NOT NULL,
    ValorLinea          DECIMAL(18,2)   NOT NULL        DEFAULT 0,
    Observaciones       NVARCHAR(500)                   NULL,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),

    CONSTRAINT PK_ReservaDetalles PRIMARY KEY (IdReservaDetalle),
    CONSTRAINT FK_ReservaDetalles_Reservas
        FOREIGN KEY (IdReserva) REFERENCES dbo.Reservas(IdReserva),
    CONSTRAINT FK_ReservaDetalles_Alojamientos
        FOREIGN KEY (IdAlojamiento) REFERENCES dbo.Alojamientos(IdAlojamiento),
    -- Un mismo alojamiento no puede aparecer dos veces en la misma reserva
    CONSTRAINT UQ_ReservaDetalles_ReservaAlojamiento
        UNIQUE (IdReserva, IdAlojamiento)
);
GO

PRINT 'Tabla ReservaDetalles creada.';

-- ============================================================
-- 18. RESERVA SERVICIOS ADICIONALES
--     Lavandería, visita día, etc. por reserva
-- ============================================================
IF OBJECT_ID('dbo.ReservaServiciosAdicionales', 'U') IS NOT NULL
    DROP TABLE dbo.ReservaServiciosAdicionales;
GO

CREATE TABLE dbo.ReservaServiciosAdicionales (
    IdReservaServicio   INT             IDENTITY(1,1)   NOT NULL,
    IdReserva           INT                             NOT NULL,
    IdServicio          INT                             NOT NULL,
    Cantidad            INT             NOT NULL        DEFAULT 1,
    ValorUnitario       DECIMAL(18,2)   NOT NULL        DEFAULT 0,
    ValorTotal          DECIMAL(18,2)   NOT NULL        DEFAULT 0,
    Observaciones       NVARCHAR(300)                   NULL,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),

    CONSTRAINT PK_ReservaServiciosAdicionales PRIMARY KEY (IdReservaServicio),
    CONSTRAINT FK_RSA_Reservas
        FOREIGN KEY (IdReserva) REFERENCES dbo.Reservas(IdReserva),
    CONSTRAINT FK_RSA_Servicios
        FOREIGN KEY (IdServicio) REFERENCES dbo.ServiciosAdicionales(IdServicio),
    CONSTRAINT CK_RSA_Cantidad CHECK (Cantidad > 0)
);
GO

PRINT 'Tabla ReservaServiciosAdicionales creada.';

-- ============================================================
-- 19. PAGOS
--     Estructura preparada para pagos en línea
-- ============================================================
IF OBJECT_ID('dbo.Pagos', 'U') IS NOT NULL
    DROP TABLE dbo.Pagos;
GO

CREATE TABLE dbo.Pagos (
    IdPago              INT             IDENTITY(1,1)   NOT NULL,
    IdReserva           INT                             NOT NULL,
    FechaPago           DATETIME2       NOT NULL        DEFAULT GETDATE(),
    ValorPagado         DECIMAL(18,2)   NOT NULL,
    -- MetodoPago: Tarjeta, PSE, Efectivo, Transferencia, etc.
    MetodoPago          NVARCHAR(50)                    NOT NULL,
    -- ReferenciaPago: código de transacción del proveedor de pagos
    ReferenciaPago      NVARCHAR(200)                   NULL,
    -- EstadoPago: Pendiente, Aprobado, Rechazado, Revertido
    EstadoPago          NVARCHAR(30)    NOT NULL        DEFAULT 'Pendiente',
    Observaciones       NVARCHAR(500)                   NULL,
    FechaCreacion       DATETIME2       NOT NULL        DEFAULT GETDATE(),
    FechaModificacion   DATETIME2                       NULL,
    UsuarioCreacion     NVARCHAR(256)                   NULL,

    CONSTRAINT PK_Pagos PRIMARY KEY (IdPago),
    CONSTRAINT FK_Pagos_Reservas
        FOREIGN KEY (IdReserva) REFERENCES dbo.Reservas(IdReserva),
    CONSTRAINT CK_Pagos_ValorPagado CHECK (ValorPagado > 0),
    CONSTRAINT CK_Pagos_EstadoPago
        CHECK (EstadoPago IN ('Pendiente','Aprobado','Rechazado','Revertido'))
);
GO

PRINT 'Tabla Pagos creada.';

PRINT '=== Todas las tablas creadas correctamente. ===';
GO
