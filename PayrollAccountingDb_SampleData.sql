
USE PayrollAccountingDb;
GO

/* ============================
   Datos de prueba para Empleados
   ============================ */
IF NOT EXISTS (SELECT 1 FROM dbo.Employees)
BEGIN
    INSERT INTO dbo.Employees (Cedula, Nombre, Departamento, Puesto, SalarioMensual, NominaId)
    VALUES 
    ('001-0000001-1', N'Juan Pérez', N'Administración', N'Contador', 45000.00, 1),
    ('001-0000002-2', N'Ana Gómez', N'Recursos Humanos', N'Analista', 38000.00, 1),
    ('001-0000003-3', N'Luis Rodríguez', N'Operaciones', N'Supervisor', 52000.00, 1);
END
GO

/* ============================
   Datos de prueba para Transacciones
   ============================ */
-- Ingresos: Sueldo
INSERT INTO dbo.PayrollTransactions (EmpleadoId, IngresoTipoId, DeduccionTipoId, TipoTransaccion, Fecha, Monto, Estado, AsientoId)
SELECT e.Id, it.Id, NULL, 1, '2025-08-05', e.SalarioMensual, 'Pendiente', NULL
FROM dbo.Employees e
JOIN dbo.IncomeTypes it ON it.Nombre = N'Sueldo';

-- Ingresos: Horas Extra (10% del salario)
INSERT INTO dbo.PayrollTransactions (EmpleadoId, IngresoTipoId, DeduccionTipoId, TipoTransaccion, Fecha, Monto, Estado, AsientoId)
SELECT e.Id, it.Id, NULL, 1, '2025-08-05', e.SalarioMensual * 0.10, 'Pendiente', NULL
FROM dbo.Employees e
JOIN dbo.IncomeTypes it ON it.Nombre = N'Horas Extra';

-- Deducciones: AFP (5% del salario)
INSERT INTO dbo.PayrollTransactions (EmpleadoId, IngresoTipoId, DeduccionTipoId, TipoTransaccion, Fecha, Monto, Estado, AsientoId)
SELECT e.Id, NULL, dt.Id, 2, '2025-08-05', e.SalarioMensual * 0.05, 'Pendiente', NULL
FROM dbo.Employees e
JOIN dbo.DeductionTypes dt ON dt.Nombre = N'AFP';

-- Deducciones: ARS (3% del salario)
INSERT INTO dbo.PayrollTransactions (EmpleadoId, IngresoTipoId, DeduccionTipoId, TipoTransaccion, Fecha, Monto, Estado, AsientoId)
SELECT e.Id, NULL, dt.Id, 2, '2025-08-05', e.SalarioMensual * 0.03, 'Pendiente', NULL
FROM dbo.Employees e
JOIN dbo.DeductionTypes dt ON dt.Nombre = N'ARS';
GO
