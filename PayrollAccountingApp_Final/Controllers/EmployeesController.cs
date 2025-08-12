using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollAccountingApp.Data;
using PayrollAccountingApp.Models;

namespace PayrollAccountingApp.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EmployeesController(ApplicationDbContext context) => _context = context;

        // GET: /Employees
        public async Task<IActionResult> Index()
        {
            var list = await _context.Employees.OrderBy(e => e.Id).ToListAsync();
            return View(list);
        }

        // GET: /Employees/Create
        public IActionResult Create() => View();

        // POST: /Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Cedula,Nombre,Departamento,Puesto,SalarioMensual,NominaId")] Employee employee)
        {
            if (!ModelState.IsValid) return View(employee);
            _context.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Employees/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: /Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cedula,Nombre,Departamento,Puesto,SalarioMensual,NominaId")] Employee employee)
        {
            if (id != employee.Id) return NotFound();
            if (!ModelState.IsValid) return View(employee);

            _context.Update(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Employees/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: /Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}