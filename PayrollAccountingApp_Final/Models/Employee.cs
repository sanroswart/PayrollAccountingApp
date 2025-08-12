using System.ComponentModel.DataAnnotations;

namespace PayrollAccountingApp.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string Cedula { get; set; } = string.Empty;

        [Required, StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(120)]
        public string? Departamento { get; set; }

        [StringLength(120)]
        public string? Puesto { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "El salario debe ser mayor que 0")]
        public decimal SalarioMensual { get; set; }

        [Range(1, int.MaxValue)]
        public int NominaId { get; set; } = 1;
    }
}