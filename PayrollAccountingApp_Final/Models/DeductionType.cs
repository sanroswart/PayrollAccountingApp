namespace PayrollAccountingApp.Models;

public class DeductionType
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool DependeDeSalario { get; set; }
    public bool Activo { get; set; } = true;

    // Mapeo contable específico para esta deducción
    public string CuentaDebe { get; set; } = "2101";   // Reverso de pasivo o cuenta puente
    public string CuentaHaber { get; set; } = "2401";  // Pasivo específico (ej. AFP/ARS/ISR por pagar)
}
