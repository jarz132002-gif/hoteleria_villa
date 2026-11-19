-- 1. Eliminar y recrear la base de datos desde cero
DROP DATABASE IF EXISTS hoteleria_villa_db;

CREATE DATABASE hoteleria_villa_db
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE hoteleria_villa_db;

-- 2. Crear las 12 tablas alineadas con EntityConfigurations.cs
CREATE TABLE TasasCambio (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FechaVigencia DATETIME(6) NOT NULL,
    MonedaOrigen VARCHAR(10) NOT NULL DEFAULT 'USD',
    MonedaDestino VARCHAR(10) NOT NULL DEFAULT 'VES',
    ValorTasa DECIMAL(18, 4) NOT NULL,
    Fuente VARCHAR(50) NOT NULL DEFAULT 'BCV'
) ENGINE=InnoDB;

CREATE TABLE TiposHabitacion (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(80) NOT NULL,
    CapacidadMaxima INT NOT NULL,
    TarifaUsd DECIMAL(18, 2) NOT NULL,
    Descripcion VARCHAR(300) NOT NULL
) ENGINE=InnoDB;

CREATE TABLE Habitaciones (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Numero VARCHAR(10) NOT NULL,
    Piso INT NOT NULL,
    TipoHabitacionId INT NOT NULL,
    Estado VARCHAR(20) NOT NULL DEFAULT 'Disponible',
    UNIQUE INDEX IX_Habitaciones_Numero (Numero),
    INDEX IX_Habitaciones_TipoHabitacionId (TipoHabitacionId),
    CONSTRAINT FK_Habitaciones_TiposHabitacion_TipoHabitacionId
        FOREIGN KEY (TipoHabitacionId) REFERENCES TiposHabitacion(Id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE Huespedes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Cedula VARCHAR(20) NOT NULL,
    Nombres VARCHAR(100) NOT NULL,
    Apellidos VARCHAR(100) NOT NULL,
    Correo VARCHAR(120) NOT NULL,
    Telefono VARCHAR(30) NOT NULL,
    Nacionalidad VARCHAR(50) NOT NULL,
    UNIQUE INDEX IX_Huespedes_Cedula (Cedula)
) ENGINE=InnoDB;

CREATE TABLE Reservas (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CodigoReserva VARCHAR(20) NOT NULL,
    HuespedTitularId INT NOT NULL,
    FechaEntrada DATETIME(6) NOT NULL,
    FechaSalida DATETIME(6) NOT NULL,
    Estado VARCHAR(20) NOT NULL DEFAULT 'Confirmada',
    UNIQUE INDEX IX_Reservas_CodigoReserva (CodigoReserva),
    INDEX IX_Reservas_HuespedTitularId (HuespedTitularId),
    CONSTRAINT FK_Reservas_Huespedes_HuespedTitularId
        FOREIGN KEY (HuespedTitularId) REFERENCES Huespedes(Id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE ReservaHabitaciones (
    ReservaId INT NOT NULL,
    HabitacionId INT NOT NULL,
    TarifaAcordadaUsd DECIMAL(18, 2) NOT NULL,
    PRIMARY KEY (ReservaId, HabitacionId),
    INDEX IX_ReservaHabitaciones_HabitacionId (HabitacionId),
    CONSTRAINT FK_ReservaHabitaciones_Reservas_ReservaId
        FOREIGN KEY (ReservaId) REFERENCES Reservas(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ReservaHabitaciones_Habitaciones_HabitacionId
        FOREIGN KEY (HabitacionId) REFERENCES Habitaciones(Id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE HuespedesPorReserva (
    ReservaId INT NOT NULL,
    HuespedId INT NOT NULL,
    EsTitular TINYINT(1) NOT NULL,
    PRIMARY KEY (ReservaId, HuespedId),
    INDEX IX_HuespedesPorReserva_HuespedId (HuespedId),
    CONSTRAINT FK_HuespedesPorReserva_Reservas_ReservaId
        FOREIGN KEY (ReservaId) REFERENCES Reservas(Id) ON DELETE CASCADE,
    CONSTRAINT FK_HuespedesPorReserva_Huespedes_HuespedId
        FOREIGN KEY (HuespedId) REFERENCES Huespedes(Id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE ServiciosAdicionales (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    PrecioUnitarioUsd DECIMAL(18, 2) NOT NULL,
    Categoria VARCHAR(50) NOT NULL
) ENGINE=InnoDB;

CREATE TABLE ConsumosReserva (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ReservaId INT NOT NULL,
    ServicioId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioAplicadoUsd DECIMAL(18, 2) NOT NULL,
    FechaConsumo DATETIME(6) NOT NULL,
    INDEX IX_ConsumosReserva_ReservaId (ReservaId),
    INDEX IX_ConsumosReserva_ServicioId (ServicioId),
    CONSTRAINT FK_ConsumosReserva_Reservas_ReservaId
        FOREIGN KEY (ReservaId) REFERENCES Reservas(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ConsumosReserva_ServiciosAdicionales_ServicioId
        FOREIGN KEY (ServicioId) REFERENCES ServiciosAdicionales(Id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE Facturas (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ReservaId INT NOT NULL,
    TasaCambioId INT NOT NULL,
    NumeroFactura VARCHAR(30) NOT NULL,
    NumeroControl VARCHAR(30) NOT NULL,
    FechaEmision DATETIME(6) NOT NULL,
    TasaBcvAplicada DECIMAL(18, 4) NOT NULL,
    SubTotalUsd DECIMAL(18, 2) NOT NULL,
    PorcentajeIva DECIMAL(5, 2) NOT NULL,
    MontoIvaBs DECIMAL(18, 2) NOT NULL,
    TotalBs DECIMAL(18, 2) NOT NULL,
    Estado VARCHAR(20) NOT NULL DEFAULT 'Emitida',
    INDEX IX_Facturas_ReservaId (ReservaId),
    INDEX IX_Facturas_TasaCambioId (TasaCambioId),
    CONSTRAINT FK_Facturas_Reservas_ReservaId
        FOREIGN KEY (ReservaId) REFERENCES Reservas(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Facturas_TasasCambio_TasaCambioId
        FOREIGN KEY (TasaCambioId) REFERENCES TasasCambio(Id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE Pagos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FacturaId INT NOT NULL,
    TasaCambioId INT NOT NULL,
    MetodoPago VARCHAR(30) NOT NULL,
    MonedaPago VARCHAR(10) NOT NULL,
    MontoOriginal DECIMAL(18, 2) NOT NULL,
    TasaAplicada DECIMAL(18, 4) NOT NULL,
    EquivalenteUsd DECIMAL(18, 2) NOT NULL,
    Referencia VARCHAR(80) NOT NULL,
    FechaPago DATETIME(6) NOT NULL,
    INDEX IX_Pagos_FacturaId (FacturaId),
    INDEX IX_Pagos_TasaCambioId (TasaCambioId),
    CONSTRAINT FK_Pagos_Facturas_FacturaId
        FOREIGN KEY (FacturaId) REFERENCES Facturas(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Pagos_TasasCambio_TasaCambioId
        FOREIGN KEY (TasaCambioId) REFERENCES TasasCambio(Id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE BitacoraHabitaciones (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    HabitacionId INT NOT NULL,
    EstadoAnterior VARCHAR(30) NOT NULL,
    EstadoNuevo VARCHAR(30) NOT NULL,
    FechaRegistro DATETIME(6) NOT NULL,
    Observacion VARCHAR(300) NOT NULL,
    INDEX IX_BitacoraHabitaciones_HabitacionId (HabitacionId),
    CONSTRAINT FK_BitacoraHabitaciones_Habitaciones_HabitacionId
        FOREIGN KEY (HabitacionId) REFERENCES Habitaciones(Id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 3. Datos de prueba (INSERT)
INSERT INTO TasasCambio (FechaVigencia, MonedaOrigen, MonedaDestino, ValorTasa, Fuente)
VALUES (UTC_DATE(), 'USD', 'VES', 45.5000, 'BCV');

INSERT INTO TiposHabitacion (Nombre, CapacidadMaxima, TarifaUsd, Descripcion)
VALUES
    ('Matrimonial', 2, 55.00, 'Habitación matrimonial estándar con baño privado'),
    ('Suite Ejecutiva', 2, 120.00, 'Suite amplia con minibar y balcón');

INSERT INTO Habitaciones (Numero, Piso, TipoHabitacionId, Estado)
VALUES
    ('101', 1, 1, 'Disponible'),
    ('301', 3, 2, 'Disponible');

INSERT INTO Huespedes (Cedula, Nombres, Apellidos, Correo, Telefono, Nacionalidad)
VALUES
    ('V-20456789', 'Oriana', 'Mendoza', 'oriana@correo.com', '0414-5550101', 'Venezolana'),
    ('V-24987654', 'Carlos', 'Rivas', 'carlos@correo.com', '0424-5550102', 'Venezolana');

INSERT INTO ServiciosAdicionales (Nombre, PrecioUnitarioUsd, Categoria)
VALUES
    ('Agua mineral', 2.50, 'Minibar'),
    ('Lavandería', 10.00, 'Lavanderia');

-- Reserva inicial (Id = 1) y Factura inicial (Id = 1) para probar GET /api/facturas/1 de inmediato
INSERT INTO Reservas (CodigoReserva, HuespedTitularId, FechaEntrada, FechaSalida, Estado)
VALUES ('HV-20261118-0001', 1, '2026-11-18 00:00:00', '2026-11-20 00:00:00', 'CheckOut');

INSERT INTO ReservaHabitaciones (ReservaId, HabitacionId, TarifaAcordadaUsd)
VALUES (1, 1, 55.00);

INSERT INTO HuespedesPorReserva (ReservaId, HuespedId, EsTitular)
VALUES (1, 1, 1);

INSERT INTO Facturas (ReservaId, TasaCambioId, NumeroFactura, NumeroControl, FechaEmision, TasaBcvAplicada, SubTotalUsd, PorcentajeIva, MontoIvaBs, TotalBs, Estado)
VALUES (1, 1, 'FAC-000001', '00-00000001', NOW(6), 45.5000, 110.00, 16.00, 800.80, 5805.80, 'Emitida');