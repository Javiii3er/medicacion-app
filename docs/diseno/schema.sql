-- =============================================
-- SISTEMA MOVIL DE MONITOREO Y CONFIRMACION
-- DE MEDICACION CON NOTIFICACIONES AUTOMATIZADAS
-- Base de datos: SQL Server
-- Version: 1.1 | Fecha: agosto 2026
-- Autor: Javier Jose Luis Rivera Perez
-- Universidad Mariano Galvez de Guatemala
-- =============================================

CREATE DATABASE SistemaMedicacion;
GO

USE SistemaMedicacion;
GO

-- =============================================
-- ENTIDAD: Usuario
-- =============================================
CREATE TABLE Usuario (
    idUsuario       INT             IDENTITY(1,1)   NOT NULL,
    nombre          VARCHAR(100)                    NOT NULL,
    apellido        VARCHAR(100)                    NOT NULL,
    correo          VARCHAR(150)                    NOT NULL,
    contrasenaHash  VARCHAR(255)                    NOT NULL,
    rol             VARCHAR(20)                     NOT NULL,
    activo          BIT                             NOT NULL    DEFAULT 1,
    fechaRegistro   DATETIME                        NOT NULL    DEFAULT GETDATE(),

    CONSTRAINT PK_Usuario           PRIMARY KEY (idUsuario),
    CONSTRAINT UQ_Usuario_Correo    UNIQUE (correo),
    CONSTRAINT CK_Usuario_Rol       CHECK (rol IN ('AdultoMayor', 'Familiar', 'Administrador'))
);
GO

-- =============================================
-- ENTIDAD: Medicamento
-- =============================================
CREATE TABLE Medicamento (
    idMedicamento   INT             IDENTITY(1,1)   NOT NULL,
    idUsuario       INT                             NOT NULL,
    nombre          VARCHAR(150)                    NOT NULL,
    dosis           DECIMAL(8,2)                    NOT NULL,
    unidad          VARCHAR(30)                     NOT NULL,
    frecuencia      VARCHAR(50)                     NOT NULL,
    notas           VARCHAR(300)                    NULL,
    activo          BIT                             NOT NULL    DEFAULT 1,
    fechaCreacion   DATETIME                        NOT NULL    DEFAULT GETDATE(),

    CONSTRAINT PK_Medicamento               PRIMARY KEY (idMedicamento),
    CONSTRAINT FK_Medicamento_Usuario       FOREIGN KEY (idUsuario)
                                            REFERENCES Usuario(idUsuario)
                                            ON DELETE CASCADE
                                            ON UPDATE CASCADE,
    CONSTRAINT CK_Medicamento_Dosis         CHECK (dosis > 0)
);
GO

-- =============================================
-- ENTIDAD: Horario
-- =============================================
CREATE TABLE Horario (
    idHorario           INT             IDENTITY(1,1)   NOT NULL,
    idMedicamento       INT                             NOT NULL,
    horaAdministracion  TIME                            NOT NULL,
    activo              BIT                             NOT NULL    DEFAULT 1,
    fechaCreacion       DATETIME                        NOT NULL    DEFAULT GETDATE(),

    CONSTRAINT PK_Horario               PRIMARY KEY (idHorario),
    CONSTRAINT FK_Horario_Medicamento   FOREIGN KEY (idMedicamento)
                                        REFERENCES Medicamento(idMedicamento)
                                        ON DELETE CASCADE
                                        ON UPDATE CASCADE
);
GO

-- =============================================
-- ENTIDAD: Confirmacion
-- =============================================
CREATE TABLE Confirmacion (
    idConfirmacion      INT             IDENTITY(1,1)   NOT NULL,
    idUsuario           INT                             NOT NULL,
    idMedicamento       INT                             NOT NULL,
    idHorario           INT                             NOT NULL,
    fechaConfirmacion   DATE                            NOT NULL,
    horaConfirmacion    TIME                            NOT NULL,
    timestampExacto     DATETIME                        NOT NULL    DEFAULT GETDATE(),

    CONSTRAINT PK_Confirmacion              PRIMARY KEY (idConfirmacion),
    CONSTRAINT FK_Confirmacion_Usuario      FOREIGN KEY (idUsuario)
                                            REFERENCES Usuario(idUsuario)
                                            ON DELETE NO ACTION
                                            ON UPDATE NO ACTION,
    CONSTRAINT FK_Confirmacion_Medicamento  FOREIGN KEY (idMedicamento)
                                            REFERENCES Medicamento(idMedicamento)
                                            ON DELETE NO ACTION
                                            ON UPDATE NO ACTION,
    CONSTRAINT FK_Confirmacion_Horario      FOREIGN KEY (idHorario)
                                            REFERENCES Horario(idHorario)
                                            ON DELETE NO ACTION
                                            ON UPDATE NO ACTION
);
GO

-- =============================================
-- ENTIDAD: Alerta
-- =============================================
CREATE TABLE Alerta (
    idAlerta        INT             IDENTITY(1,1)   NOT NULL,
    idUsuario       INT                             NOT NULL,
    idMedicamento   INT                             NOT NULL,
    idHorario       INT                             NOT NULL,
    horaProgramada  TIME                            NOT NULL,
    horaVencimiento DATETIME                        NOT NULL,
    estado          VARCHAR(20)                     NOT NULL    DEFAULT 'pendiente',
    horaEnvio       DATETIME                        NULL,
    fechaCreacion   DATETIME                        NOT NULL    DEFAULT GETDATE(),

    CONSTRAINT PK_Alerta                PRIMARY KEY (idAlerta),
    CONSTRAINT FK_Alerta_Usuario        FOREIGN KEY (idUsuario)
                                        REFERENCES Usuario(idUsuario)
                                        ON DELETE NO ACTION
                                        ON UPDATE NO ACTION,
    CONSTRAINT FK_Alerta_Medicamento    FOREIGN KEY (idMedicamento)
                                        REFERENCES Medicamento(idMedicamento)
                                        ON DELETE NO ACTION
                                        ON UPDATE NO ACTION,
    CONSTRAINT FK_Alerta_Horario        FOREIGN KEY (idHorario)
                                        REFERENCES Horario(idHorario)
                                        ON DELETE NO ACTION
                                        ON UPDATE NO ACTION,
    CONSTRAINT CK_Alerta_Estado         CHECK (estado IN ('pendiente', 'enviada', 'error'))
);
GO

-- =============================================
-- ENTIDAD: ContactoFamiliar
-- =============================================
CREATE TABLE ContactoFamiliar (
    idContacto      INT             IDENTITY(1,1)   NOT NULL,
    idUsuario       INT                             NOT NULL,
    nombreFamiliar  VARCHAR(150)                    NOT NULL,
    correoFamiliar  VARCHAR(150)                    NOT NULL,
    telefonoFamiliar VARCHAR(20)                    NULL,
    activo          BIT                             NOT NULL    DEFAULT 1,

    CONSTRAINT PK_ContactoFamiliar          PRIMARY KEY (idContacto),
    CONSTRAINT FK_ContactoFamiliar_Usuario  FOREIGN KEY (idUsuario)
                                            REFERENCES Usuario(idUsuario)
                                            ON DELETE CASCADE
                                            ON UPDATE CASCADE,
    CONSTRAINT UQ_ContactoFamiliar_Usuario  UNIQUE (idUsuario)
);
GO

-- =============================================
-- INDICES DE OPTIMIZACION
-- =============================================
CREATE INDEX IX_Medicamento_Usuario
    ON Medicamento(idUsuario);
GO

CREATE INDEX IX_Horario_Medicamento
    ON Horario(idMedicamento);
GO

CREATE INDEX IX_Confirmacion_Usuario_Fecha
    ON Confirmacion(idUsuario, fechaConfirmacion);
GO

CREATE INDEX IX_Confirmacion_Horario_Fecha
    ON Confirmacion(idHorario, fechaConfirmacion);
GO

CREATE INDEX IX_Alerta_Usuario_Estado
    ON Alerta(idUsuario, estado);
GO

CREATE INDEX IX_Alerta_Horario
    ON Alerta(idHorario);
GO

PRINT 'Base de datos SistemaMedicacion creada exitosamente.';
GO
