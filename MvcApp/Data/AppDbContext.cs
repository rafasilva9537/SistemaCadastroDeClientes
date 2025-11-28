using Microsoft.EntityFrameworkCore;
using MvcApp.Models;

namespace MvcApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Fornecedor> Fornecedors { get; set; }
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
            var segmentos = new List<Segmento>
            {
                new() { Nome = "Comércio" },
                new() { Nome = "Serviços" },
                new() { Nome = "Indústria" },
            };

            foreach (var segmento in segmentos)
            {
                bool segmentoExiste = context.Set<Segmento>().Any(s => s.Nome == segmento.Nome);
                if (!segmentoExiste)
                {
                    context.Set<Segmento>().Add(segmento);
                }
            }

            context.SaveChanges();
        });
    }
}
