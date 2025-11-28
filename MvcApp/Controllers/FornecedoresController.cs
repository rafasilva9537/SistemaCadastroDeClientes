using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcApp.Data;
using MvcApp.Mappers;
using MvcApp.ViewModels.Fornecedor;

namespace MvcApp.Controllers;

public class FornecedoresController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<FornecedoresController> _logger;

    public FornecedoresController(AppDbContext dbContext, ILogger<FornecedoresController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var fornecedoresViewModel = await _dbContext.Fornecedores
            .Select(FornecedorMappers.ProjectToFornecedorViewModel)
            .ToListAsync();

        return View(fornecedoresViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Criar()
    {
        var segmentosViewModels = await _dbContext.Segmentos
            .Select(SegmentoMappers.ProjectToSegmentoViewModel)
            .ToListAsync();

        var criarFornecedorViewModel = new CriarFornecedorFormViewModel
        {
            Segmentos = segmentosViewModels
        };

        return View(criarFornecedorViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CriarFornecedorFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            form.Segmentos = await _dbContext.Segmentos
                .Select(SegmentoMappers.ProjectToSegmentoViewModel)
                .ToListAsync();

            return View(form);
        }

        var fornecedorModel = form.Fornecedor.ToFornecedorModel();
        _dbContext.Fornecedores.Add(fornecedorModel);
        await _dbContext.SaveChangesAsync();
        
        return RedirectToAction(nameof(Criado), new { idPublico = fornecedorModel.IdPublico });
    }

    [HttpGet]
    public async Task<IActionResult> Criado(Guid idPublico)
    {
        var fornecedor = await _dbContext.Fornecedores
            .Where(f => f.IdPublico == idPublico)
            .Select(FornecedorMappers.ProjectToFornecedorViewModel)
            .FirstOrDefaultAsync();

        if (fornecedor is null)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(fornecedor);
    }

    [HttpGet]
    public async Task<IActionResult> Editar(Guid idPublico)
    {
        var fornecedor = await _dbContext.Fornecedores
            .Where(f => f.IdPublico == idPublico)
            .FirstOrDefaultAsync();

        if (fornecedor is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var segmentosViewModels = await _dbContext.Segmentos
            .Select(SegmentoMappers.ProjectToSegmentoViewModel)
            .ToListAsync();

        var viewModel = fornecedor.ToEditarFornecedorViewModel(segmentosViewModels);

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(EditarFornecedorViewModel form)
    {
        if (!ModelState.IsValid)
        {
            form.Segmentos = await _dbContext.Segmentos
                .Select(SegmentoMappers.ProjectToSegmentoViewModel)
                .ToListAsync();

            return View(form);
        }

        var fornecedor = await _dbContext.Fornecedores
            .Where(f => f.IdPublico == form.IdPublico)
            .FirstOrDefaultAsync();

        if (fornecedor is null)
        {
            return RedirectToAction(nameof(Index));
        }

        fornecedor.Nome = form.Nome;
        fornecedor.Cnpj = form.Cnpj;
        fornecedor.Cep = form.Cep;
        fornecedor.SegmentoId = form.SegmentoId;

        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Criado), new { idPublico = fornecedor.IdPublico });
    }

    public IActionResult Deletar(int id)
    {
        return RedirectToAction("Index");
    }
}
