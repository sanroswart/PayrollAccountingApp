using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollAccountingApp.Data;
using PayrollAccountingApp.Models;

namespace PayrollAccountingApp.Controllers;

public class DeductionTypesController : Controller
{
    private readonly ApplicationDbContext _db;
    public DeductionTypesController(ApplicationDbContext db) { _db = db; }

    public async Task<IActionResult> Index() => View(await _db.DeductionTypes.ToListAsync());

    public IActionResult Create() => View(new DeductionType());
    [HttpPost]
    public async Task<IActionResult> Create(DeductionType m)
    {
        if (!ModelState.IsValid) return View(m);
        _db.Add(m); await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var m = await _db.DeductionTypes.FindAsync(id);
        if (m == null) return NotFound();
        return View(m);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, DeductionType m)
    {
        if (id != m.Id) return BadRequest();
        if (!ModelState.IsValid) return View(m);
        _db.Update(m); await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var m = await _db.DeductionTypes.FindAsync(id);
        if (m == null) return NotFound();
        return View(m);
    }
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var m = await _db.DeductionTypes.FindAsync(id);
        if (m != null) { _db.Remove(m); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
