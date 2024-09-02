using Microsoft.EntityFrameworkCore;

namespace Bank.Loan.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(typeof(Ref).Assembly);
    
    public DbSet<Domain.Loan> Loans { get; set; }
}