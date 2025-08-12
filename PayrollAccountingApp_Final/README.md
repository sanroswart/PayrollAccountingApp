# PayrollAccountingApp (ASP.NET Core 8 + EF Core)

**Qué hace**
- Gestiona entidades mínimas de nómina (empleados, tipos y transacciones).
- Pantalla para **Enviar a Contabilidad** con filtros por fecha y tabla tal como el boceto.
- Genera asientos por día y los **envía** al sistema contable externo mediante `x-api-key`.

**Configurar**
1. En `appsettings.json` cambia `DefaultConnection` a tu SQL Server.
2. Coloca la **API Key de Nóminas** en `AccountingApi:ApiKey`.
3. Opcional: cambia `DebitAccount` y `CreditAccount` según el mapeo.

**Migraciones**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

**Notas**
- En `Services/AccountingApiClient.cs` se implementa `POST /api/public/entradas-contables`.
- El payload cumple con el contrato: `{ fecha, descripcion, movimientos[{ cuenta, debe, haber }] }`.
- La API base y la cabecera `x-api-key` se leen de configuración.



## Mapeo por Tipo de Ingreso/Deducción
- En **Models/IncomeType.cs** y **Models/DeductionType.cs** agrega las cuentas `CuentaDebe` y `CuentaHaber` por cada tipo.
- El controlador **TransactionsController.Contabilizar** agrupa por fecha y por par de cuentas, generando líneas separadas por tipo.
- Así cada tipo puede impactar cuentas diferentes (ej. Sueldos, Horas Extra, Comisiones; AFP, ARS, ISR, etc.).


## CRUD incluidos
- /Employees (Empleados)
- /IncomeTypes (Tipos de Ingreso)
- /DeductionTypes (Tipos de Deducción)
- /PayrollTransactions (Transacciones)

## Contabilización
- Ingresos: usa `AccountingApi:DebitAccount` y `AccountingApi:CreditAccount` (globales).
- Deducciones: usa las cuentas definidas por cada **Tipo de Deducción**.

## Base de datos
- Configura `ConnectionStrings:DefaultConnection` y ejecuta migraciones con `dotnet ef`.
