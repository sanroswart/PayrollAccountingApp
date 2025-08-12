namespace PayrollAccountingApp.Models;

public class IncomeType
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool DependeDeSalario { get; set; }
    public bool Activo { get; set; } = true;
}
