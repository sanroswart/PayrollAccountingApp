
/* =========================================================
   PayrollAccountingDb - Manual SQL Schema (SQL Server)
   Compatible with the ASP.NET Core + EF Core project provided.
   ========================================================= */

-- 1) Create database (optional; adjust file locations as needed)
IF DB_ID(N'PayrollAccountingDb') IS NULL
BEGIN
  CREATE DATABASE PayrollAccountingDb;
END
GO

USE PayrollAccountingDb;
GO

-- 2) Tables

-- Employees
IF OBJECT_ID(N'dbo.Employees', N'U') IS NULL
BEGIN
CREATE TABLE dbo.Employees (
    Id               INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Employees PRIMARY KEY,
    Cedula           NVARCHAR(20)  NOT NULL,
    Nombre           NVARCHAR(120) NOT NULL,
    Departamento     NVARCHAR(120) NULL,
    Puesto           NVARCHAR(120) NULL,
    SalarioMensual   DECIMAL(18,2) NOT NULL CONSTRAINT DF_Employees_Salario DEFAULT(0),
    NominaId         INT NOT NULL CONSTRAINT DF_Employees_NominaId DEFAULT(1)
);
CREATE UNIQUE INDEX UX_Employees_Cedula ON dbo.Employees(Cedula);
END
GO

-- IncomeTypes (Tipos de Ingreso) - per PPT (sin cuentas)
IF OBJECT_ID(N'dbo.IncomeTypes', N'U') IS NULL
BEGIN
CREATE TABLE dbo.IncomeTypes (
    Id                 INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_IncomeTypes PRIMARY KEY,
    Nombre             NVARCHAR(120) NOT NULL,
    DependeDeSalario   BIT NOT NULL CONSTRAINT DF_IncomeTypes_Depende DEFAULT(0),
    Activo             BIT NOT NULL CONSTRAINT DF_IncomeTypes_Activo DEFAULT(1)
);
CREATE UNIQUE INDEX UX_IncomeTypes_Nombre ON dbo.IncomeTypes(Nombre);
END
GO

-- DeductionTypes (Tipos de Deducción) - extendido con cuentas
IF OBJECT_ID(N'dbo.DeductionTypes', N'U') IS NULL
BEGIN
CREATE TABLE dbo.DeductionTypes (
    Id                 INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DeductionTypes PRIMARY KEY,
    Nombre             NVARCHAR(120) NOT NULL,
    DependeDeSalario   BIT NOT NULL CONSTRAINT DF_DeductionTypes_Depende DEFAULT(0),
    Activo             BIT NOT NULL CONSTRAINT DF_DeductionTypes_Activo DEFAULT(1),
    CuentaDebe         NVARCHAR(20) NOT NULL CONSTRAINT DF_DeductionTypes_CtaDebe DEFAULT(N'2101'),
    CuentaHaber        NVARCHAR(20) NOT NULL CONSTRAINT DF_DeductionTypes_CtaHaber DEFAULT(N'2401')
);
CREATE UNIQUE INDEX UX_DeductionTypes_Nombre ON dbo.DeductionTypes(Nombre);
END
GO

-- JournalEntries (Asientos)
IF OBJECT_ID(N'dbo.JournalEntries', N'U') IS NULL
BEGIN
CREATE TABLE dbo.JournalEntries (
    Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_JournalEntries PRIMARY KEY,
    FechaAsiento  DATE NOT NULL,
    Descripcion   NVARCHAR(200) NOT NULL,
    Estado        NVARCHAR(60)  NOT NULL CONSTRAINT DF_JournalEntries_Estado DEFAULT(N'Generado'),
    ExternalId    NVARCHAR(100) NULL
);
CREATE INDEX IX_JournalEntries_Fecha ON dbo.JournalEntries(FechaAsiento);
END
GO

-- JournalEntryLines (Movimientos)
IF OBJECT_ID(N'dbo.JournalEntryLines', N'U') IS NULL
BEGIN
CREATE TABLE dbo.JournalEntryLines (
    Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_JournalEntryLines PRIMARY KEY,
    JournalEntryId  INT NOT NULL,
    Cuenta          NVARCHAR(20) NOT NULL,
    Debe            DECIMAL(18,2) NOT NULL CONSTRAINT DF_JournalEntryLines_Debe DEFAULT(0),
    Haber           DECIMAL(18,2) NOT NULL CONSTRAINT DF_JournalEntryLines_Haber DEFAULT(0),
    CONSTRAINT CK_JournalEntryLines_Positive CHECK (Debe >= 0 AND Haber >= 0),
    CONSTRAINT CK_JournalEntryLines_OneSide CHECK ((Debe = 0 AND Haber > 0) OR (Haber = 0 AND Debe > 0))
);
ALTER TABLE dbo.JournalEntryLines
  ADD CONSTRAINT FK_JournalEntryLines_JournalEntries
      FOREIGN KEY (JournalEntryId) REFERENCES dbo.JournalEntries(Id) ON DELETE CASCADE;
CREATE INDEX IX_JournalEntryLines_JournalEntryId ON dbo.JournalEntryLines(JournalEntryId);
CREATE INDEX IX_JournalEntryLines_Cuenta ON dbo.JournalEntryLines(Cuenta);
END
GO

-- PayrollTransactions (Transacciones de Nómina)
IF OBJECT_ID(N'dbo.PayrollTransactions', N'U') IS NULL
BEGIN
CREATE TABLE dbo.PayrollTransactions (
    Id                INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PayrollTransactions PRIMARY KEY,
    EmpleadoId        INT NOT NULL,
    IngresoTipoId     INT NULL,
    DeduccionTipoId   INT NULL,
    TipoTransaccion   INT NOT NULL, -- 1=Ingreso, 2=Deduccion
    Fecha             DATE NOT NULL,
    Monto             DECIMAL(18,2) NOT NULL CONSTRAINT DF_PayrollTransactions_Monto DEFAULT(0),
    Estado            NVARCHAR(40) NOT NULL CONSTRAINT DF_PayrollTransactions_Estado DEFAULT(N'Pendiente'),
    AsientoId         INT NULL,
    CONSTRAINT CK_PayrollTransactions_Tipo CHECK (TipoTransaccion IN (1,2)),
    CONSTRAINT CK_PayrollTransactions_Monto CHECK (Monto > 0)
);
ALTER TABLE dbo.PayrollTransactions
  ADD CONSTRAINT FK_PayrollTransactions_Employees
      FOREIGN KEY (EmpleadoId) REFERENCES dbo.Employees(Id);

ALTER TABLE dbo.PayrollTransactions
  ADD CONSTRAINT FK_PayrollTransactions_IncomeTypes
      FOREIGN KEY (IngresoTipoId) REFERENCES dbo.IncomeTypes(Id);

ALTER TABLE dbo.PayrollTransactions
  ADD CONSTRAINT FK_PayrollTransactions_DeductionTypes
      FOREIGN KEY (DeduccionTipoId) REFERENCES dbo.DeductionTypes(Id);

ALTER TABLE dbo.PayrollTransactions
  ADD CONSTRAINT FK_PayrollTransactions_JournalEntries
      FOREIGN KEY (AsientoId) REFERENCES dbo.JournalEntries(Id) ON DELETE SET NULL;

CREATE INDEX IX_PayrollTransactions_Fecha ON dbo.PayrollTransactions(Fecha);
CREATE INDEX IX_PayrollTransactions_Empleado ON dbo.PayrollTransactions(EmpleadoId);
CREATE INDEX IX_PayrollTransactions_Tipo ON dbo.PayrollTransactions(TipoTransaccion);
END
GO

/* 3) Opcional: datos básicos de catálogo */
IF NOT EXISTS (SELECT 1 FROM dbo.IncomeTypes)
BEGIN
  INSERT INTO dbo.IncomeTypes (Nombre, DependeDeSalario, Activo)
  VALUES (N'Sueldo', 1, 1),
         (N'Horas Extra', 1, 1),
         (N'Comisiones', 0, 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DeductionTypes)
BEGIN
  INSERT INTO dbo.DeductionTypes (Nombre, DependeDeSalario, Activo, CuentaDebe, CuentaHaber)
  VALUES (N'AFP', 1, 1, N'2101', N'2401'),
         (N'ARS', 1, 1, N'2101', N'2402'),
         (N'ISR', 0, 1, N'2101', N'2403');
END
GO
