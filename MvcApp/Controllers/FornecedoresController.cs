using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcApp.Data;
using MvcApp.Mappers;
using MvcApp.ViewModels;

namespace MvcApp.Controllers;

public class FornecedoresController : Controller
{
    private readonly AppDbContext _dbContext;

    public FornecedoresController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var fornecedoresViewModel = await _dbContext.Fornecedores
            .Select(FornecedoresMappers.ProjectToFornecedorViewModel)
            .ToListAsync();

        return View(fornecedoresViewModel);
    }

    [HttpPost]
    public IActionResult Criar(CriarFornecedorViewModel fornecedor)
    {
        return View(fornecedor);
    }

    public IActionResult Editar(int id)
    {
        return View();
    }

    public IActionResult Deletar(int id)
    {
        return RedirectToAction("Index");
    }
}
