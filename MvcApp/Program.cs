using Microsoft.EntityFrameworkCore;
using MvcApp.Configs;
using MvcApp.Constantes;
using MvcApp.Data;
using MvcApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSqlServer<AppDbContext>(connectionString);

builder.Services.AddOptions<ViaCepConfigs>()
    .BindConfiguration(ViaCepConfigs.NomeSection)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHttpClient(ServicosExternosConstantes.ClienteViaCep, (httpClient) =>
{
    string? urlBase = builder.Configuration
        .GetSection(ViaCepConfigs.NomeSection)
        .Get<ViaCepConfigs>()?.UrlBase;
    if (urlBase is null)
    {
        throw new InvalidOperationException("Não foi possível obter a url base de acesso ao ViaCep. Este erro não deveria ser possível com a validação em TOptions.");
    }
    
    httpClient.BaseAddress = new Uri(urlBase);
});

builder.Services.AddScoped<IImagemService, ImagemLocalService>();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Fornecedores}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
