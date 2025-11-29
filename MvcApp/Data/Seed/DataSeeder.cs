using Bogus;
using Microsoft.EntityFrameworkCore;
using MvcApp.Models;

namespace MvcApp.Data.Seed;

public static class DataSeeder
{
    public static void SeedSegmentos(DbContext context)
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
    }

    public static void SeedFornecedores(DbContext context, int quantidade = 50)
    {
        if (context.Set<Fornecedor>().Any()) return;

        var segmentos = context.Set<Segmento>().ToList();

        var fornecedorFaker = new Faker<Fornecedor>("pt_BR")
            .UseSeed(124)
            .RuleFor(f => f.Nome, f => f.Company.CompanyName())
            .RuleFor(f => f.Cnpj, f => f.Random.Replace("########0001##"))
            .RuleFor(f => f.Cep, f => f.Address.ZipCode("########"))
            .RuleFor(f => f.Endereco, f => 
                {
                    string? logradouro = f.Address.StreetAddress();
                    string? complemento = f.Address.SecondaryAddress();
                    string? bairro = f.Address.StreetName();
                    string? cidade = f.Address.City();
                    string? uf = f.Address.StateAbbr();
                    return $"{logradouro}, {complemento} - {bairro} - {cidade}/{uf}";
                })
            .RuleFor(f => f.SegmentoId, f => f.PickRandom(segmentos).Id);

        var fornecedores = fornecedorFaker.Generate(quantidade);
        foreach (var fornecedor in fornecedores)
        {
            bool fornecedorExiste = context.Set<Fornecedor>().Any(f => f.Cnpj == fornecedor.Cnpj);
            if (!fornecedorExiste)
            {
                context.Set<Fornecedor>().Add(fornecedor);
            }
        }

        context.SaveChanges();
    }
}
