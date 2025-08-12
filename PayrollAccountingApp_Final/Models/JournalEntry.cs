using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollAccountingApp.Models;

public class JournalEntry
{
    public int Id { get; set; }
    public DateTime FechaAsiento { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = "Generado";
    public ICollection<JournalEntryLine> Movimientos { get; set; } = new List<JournalEntryLine>();
    public string? ExternalId { get; set; } // id devuelto por contabilidad
}

public class JournalEntryLine
{
    public int Id { get; set; }
    public int JournalEntryId { get; set; }
    public JournalEntry? JournalEntry { get; set; }
    public string Cuenta { get; set; } = string.Empty;
    [Column(TypeName="decimal(18,2)")]
    public decimal Debe { get; set; }
    [Column(TypeName="decimal(18,2)")]
    public decimal Haber { get; set; }
}
