using Microsoft.EntityFrameworkCore;
using PayrollAccountingApp.Models;

namespace PayrollAccountingApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<IncomeType> IncomeTypes => Set<IncomeType>();
    public DbSet<DeductionType> DeductionTypes => Set<DeductionType>();
    public DbSet<PayrollTransaction> PayrollTransactions => Set<PayrollTransaction>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PayrollTransaction>()
            .HasOne(t => t.Asiento)
            .WithMany()
            .HasForeignKey(t => t.AsientoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
