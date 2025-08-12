using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PayrollAccountingApp.Data;
using PayrollAccountingApp.Models;
using PayrollAccountingApp.Services;

namespace PayrollAccountingApp.Controllers;

public class TransactionsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly AccountingApiClient _api;
    private readonly AccountingApiOptions _opts;

    public TransactionsController(ApplicationDbContext db, AccountingApiClient api, IOptions<AccountingApiOptions> opts)
    {
        _db = db;
        _api = api;
        _opts = opts.Value;
    }

    // GET: /Transactions
    public async Task<IActionResult> Index(DateTime? desde, DateTime? hasta)
    {
        var query = _db.PayrollTransactions
            .Include(t => t.Empleado)
            .Include(t => t.IngresoTipo)
            .Include(t => t.DeduccionTipo)
            .OrderBy(t => t.Fecha)
            .AsQueryable();

        if (desde.HasValue) query = query.Where(t => t.Fecha >= desde.Value);
        if (hasta.HasValue) query = query.Where(t => t.Fecha <= hasta.Value);

        var list = await query.ToListAsync();
        return View(list);
    }

    // POST: /Transactions/Contabilizar
    [HttpPost]
    public async Task<IActionResult> Contabilizar(DateTime? desde, DateTime? hasta)
    {
        var pend = _db.PayrollTransactions
            .Include(t => t.IngresoTipo)
            .Include(t => t.DeduccionTipo)
            .Where(t => t.AsientoId == null);

        if (desde.HasValue) pend = pend.Where(t => t.Fecha >= desde.Value);
        if (hasta.HasValue) pend = pend.Where(t => t.Fecha <= hasta.Value);

        var transactions = await pend.ToListAsync();
        if (!transactions.Any())
        {
            TempData["Msg"] = "No hay transacciones pendientes en el rango.";
            return RedirectToAction(nameof(Index), new { desde, hasta });
        }

        // Un asiento por día
        var grupos = transactions.GroupBy(t => t.Fecha.Date);

        foreach (var g in grupos)
        {
            var movimientos = new List<JournalEntryLine>();

            // 1) INGRESOS: consolidar en cuentas globales (DebitAccount / CreditAccount)
            var totalIngresos = g
                .Where(x => x.TipoTransaccion == TransactionKind.Ingreso)
                .Sum(x => x.Monto);

            if (totalIngresos > 0)
            {
                movimientos.Add(new JournalEntryLine
                {
                    Cuenta = _opts.DebitAccount,
                    Debe = totalIngresos,
                    Haber = 0
                });
                movimientos.Add(new JournalEntryLine
                {
                    Cuenta = _opts.CreditAccount,
                    Debe = 0,
                    Haber = totalIngresos
                });
            }

            // 2) DEDUCCIONES: por tipo, usando las cuentas del tipo (CuentaDebe/CuentaHaber)
            var dedPorCuenta = g
                .Where(x => x.TipoTransaccion == TransactionKind.Deduccion && x.DeduccionTipo != null)
                .GroupBy(x => new { x.DeduccionTipo!.CuentaDebe, x.DeduccionTipo!.CuentaHaber })
                .Select(grp => new
                {
                    grp.Key.CuentaDebe,
                    grp.Key.CuentaHaber,
                    Monto = grp.Sum(x => x.Monto)
                });

            foreach (var dt in dedPorCuenta)
            {
                if (dt.Monto <= 0) continue;
                movimientos.Add(new JournalEntryLine { Cuenta = dt.CuentaDebe, Debe = dt.Monto, Haber = 0 });
                movimientos.Add(new JournalEntryLine { Cuenta = dt.CuentaHaber, Debe = 0, Haber = dt.Monto });
            }

            if (!movimientos.Any())
                continue;

            var entry = new JournalEntry
            {
                FechaAsiento = g.Key,
                Descripcion = $"Nómina del {g.Key:dd/MM/yyyy}",
                Movimientos = movimientos
            };

            _db.JournalEntries.Add(entry);
            await _db.SaveChangesAsync();

            try
            {
                var responseId = await _api.CrearEntradaContableAsync(entry);
                entry.ExternalId = responseId;
                entry.Estado = "Enviado";

                foreach (var t in g)
                {
                    t.AsientoId = entry.Id;
                    t.Estado = "Contabilizado";
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                entry.Estado = $"Error: {ex.Message}";
                await _db.SaveChangesAsync();
            }
        }

        TempData["Msg"] = "Proceso de contabilización ejecutado.";
        return RedirectToAction(nameof(Index), new { desde, hasta });
    }
}
