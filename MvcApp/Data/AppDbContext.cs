using Microsoft.EntityFrameworkCore;
using MvcApp.Data.Seed;
using MvcApp.Models;

namespace MvcApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Fornecedor> Fornecedores { get; set; }
    public DbSet<Segmento> Segmentos { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSeeding((context, _) =>
        {
            DataSeeder.SeedSegmentos(context);
            DataSeeder.SeedFornecedores(context, quantidade: 15);
        });
    }
}
