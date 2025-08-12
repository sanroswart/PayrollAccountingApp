using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollAccountingApp.Models;

public enum TransactionKind { Ingreso = 1, Deduccion = 2 }

public class PayrollTransaction
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public Employee? Empleado { get; set; }
    public int? IngresoTipoId { get; set; }
    public IncomeType? IngresoTipo { get; set; }
    public int? DeduccionTipoId { get; set; }
    public DeductionType? DeduccionTipo { get; set; }
    public TransactionKind TipoTransaccion { get; set; }
    public DateTime Fecha { get; set; }
    [Column(TypeName="decimal(18,2)")]
    public decimal Monto { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public int? AsientoId { get; set; }
    public JournalEntry? Asiento { get; set; }
    public string Descripcion => TipoTransaccion == TransactionKind.Ingreso 
        ? $"Ingreso: {IngresoTipo?.Nombre}" 
        : $"Deducción: {DeduccionTipo?.Nombre}";
}
