using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PayrollAccountingApp.Data;
using PayrollAccountingApp.Models;

namespace PayrollAccountingApp.Controllers;

public class PayrollTransactionsController : Controller
{
    private readonly ApplicationDbContext _db;
    public PayrollTransactionsController(ApplicationDbContext db) { _db = db; }

    public async Task<IActionResult> Index()
    {
        var list = await _db.PayrollTransactions
            .Include(t=>t.Empleado)
            .Include(t=>t.IngresoTipo)
            .Include(t=>t.DeduccionTipo)
            .OrderByDescending(t=>t.Fecha)
            .ToListAsync();
        return View(list);
    }

    private void LoadViewBags()
    {
        ViewBag.Employees = new SelectList(_db.Employees.OrderBy(e=>e.Nombre), "Id", "Nombre");
        ViewBag.IncomeTypes = new SelectList(_db.IncomeTypes.OrderBy(i=>i.Nombre), "Id", "Nombre");
        ViewBag.DeductionTypes = new SelectList(_db.DeductionTypes.OrderBy(d=>d.Nombre), "Id", "Nombre");
    }

    public IActionResult Create()
    {
        LoadViewBags();
        return View(new PayrollTransaction { Fecha = DateTime.Today, TipoTransaccion = TransactionKind.Ingreso });
    }

    [HttpPost]
    public async Task<IActionResult> Create(PayrollTransaction m)
    {
        if (!ModelState.IsValid)
        {
            LoadViewBags();
            return View(m);
        }
        _db.Add(m); await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var m = await _db.PayrollTransactions.FindAsync(id);
        if (m == null) return NotFound();
        LoadViewBags();
        return View(m);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, PayrollTransaction m)
    {
        if (id != m.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            LoadViewBags();
            return View(m);
        }
        _db.Update(m); await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var m = await _db.PayrollTransactions
            .Include(t=>t.Empleado)
            .FirstOrDefaultAsync(t=>t.Id==id);
        if (m == null) return NotFound();
        return View(m);
    }
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var m = await _db.PayrollTransactions.FindAsync(id);
        if (m != null) { _db.Remove(m); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
