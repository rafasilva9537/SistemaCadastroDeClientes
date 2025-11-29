using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MvcApp.Constantes;
using MvcApp.Data;
using MvcApp.Dtos;
using MvcApp.Helper;
using MvcApp.Mappers;
using MvcApp.Services;
using MvcApp.ViewModels.Fornecedor;

namespace MvcApp.Controllers;

public class FornecedoresController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<FornecedoresController> _logger;
    private readonly IImagemService _imagemService;

    public FornecedoresController(
        AppDbContext dbContext, 
        ILogger<FornecedoresController> logger,
        IImagemService imagemService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _imagemService = imagemService;
    }

    public async Task<IActionResult> Index()
    {
        var fornecedoresViewModel = await _dbContext.Fornecedores
            .OrderByDescending(f => f.Id)
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

        var criarFornecedorViewModel = new CriarFornecedorViewModel
        {
            Segmentos = segmentosViewModels
        };

        return View(criarFornecedorViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(
        CriarFornecedorViewModel criarFornecedorViewModel, 
        [FromServices] IHttpClientFactory httpClientFactory)
    {
        bool cpnjExiste = await _dbContext.Fornecedores
            .AnyAsync(f => f.Cnpj == criarFornecedorViewModel.Cnpj);

        if (cpnjExiste)
        {
            ModelState.AddModelError(
                nameof(criarFornecedorViewModel.Cnpj),
                "Já existe um fornecedor cadastrado com este CNPJ.");
        }
   
        if (!ModelState.IsValid)
        {
            criarFornecedorViewModel.Segmentos = await _dbContext.Segmentos
                .Select(SegmentoMappers.ProjectToSegmentoViewModel)
                .ToListAsync();

            return View(criarFornecedorViewModel);
        }

        var fornecedor = criarFornecedorViewModel.ToFornecedorModel();

        string? endereco = await ObterEnderecoOuAdiconarErroAsync(fornecedor.Cep, httpClientFactory, ModelState);
        if (!ModelState.IsValid || endereco is null)
        {
            // Se deu algum erro na consulta do CEP, voltamos para a tela
            criarFornecedorViewModel.Segmentos = await _dbContext.Segmentos
                .Select(SegmentoMappers.ProjectToSegmentoViewModel)
                .ToListAsync();

            return View(criarFornecedorViewModel);
        }

        fornecedor.Endereco = endereco;
        
        // Upload da foto de perfil, se informado
        const int tamanhoMaximoBytes = 1024 * 1024 * 3; // 3MB
        if (criarFornecedorViewModel.FotoPerfil is { Length: > 0 and < tamanhoMaximoBytes })
        {
            var fotoPerfil = criarFornecedorViewModel.FotoPerfil;
            string caminhoRelativoImagem = await _imagemService.SalvarImagemAsync(fotoPerfil, "fornecedores");
            fornecedor.FotoPerfilPath = caminhoRelativoImagem;
            _logger.LogInformation("Foto de perfil salva com sucesso: {NomeImagem}", caminhoRelativoImagem);
        }
        
        _dbContext.Fornecedores.Add(fornecedor);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Fornecedor criado com sucesso. Nome:{Nome}, IdPublico: {IdPublico}, CNPJ: {Cnpj}",
            fornecedor.Nome,
            fornecedor.IdPublico,
            fornecedor.Cnpj);

        return RedirectToAction(nameof(Criado), new { idPublico = fornecedor.IdPublico });
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
            _logger.LogWarning("Fornecedor não encontrado ao exibir tela de criado. IdPublico: {IdPublico}", 
                idPublico);

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
            _logger.LogWarning("Fornecedor não encontrado para edição. IdPublico: {IdPublico}", idPublico);

            return RedirectToAction(nameof(Index));
        }

        var segmentosViewModels = await _dbContext.Segmentos
            .Select(SegmentoMappers.ProjectToSegmentoViewModel)
            .ToListAsync();

        var viewModel = fornecedor.ToEditarFornecedorViewModel(segmentosViewModels);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        EditarFornecedorViewModel form, 
        [FromServices] IHttpClientFactory httpClientFactory)
    {
        bool cnpjExiste = await _dbContext.Fornecedores
            .AnyAsync(f => f.Cnpj == form.Cnpj && f.IdPublico != form.IdPublico);

        if (cnpjExiste)
        {
            ModelState.AddModelError(
                nameof(form.Cnpj),
                "Já existe outro fornecedor cadastrado com este CNPJ.");
        }
    
        string? novoEndereco = await ObterEnderecoOuAdiconarErroAsync(form.Cep, httpClientFactory, ModelState);
        if (!ModelState.IsValid || novoEndereco is null)
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
            _logger.LogWarning(
                "Fornecedor não encontrado ao tentar salvar edição. IdPublico: {IdPublico}",
                form.IdPublico);

            return RedirectToAction(nameof(Index));
        }
    
        fornecedor.Cep = form.Cep;
        fornecedor.Nome = form.Nome;
        fornecedor.Cnpj = form.Cnpj;
        fornecedor.SegmentoId = form.SegmentoId;
        fornecedor.Endereco = novoEndereco ?? fornecedor.Endereco;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Fornecedor editado com sucesso. IdPublico: {IdPublico}, CNPJ: {Cnpj}",
            fornecedor.IdPublico,
            fornecedor.Cnpj);

        return RedirectToAction(nameof(Criado), new { idPublico = fornecedor.IdPublico });
    }

    [HttpGet]
    public async Task<IActionResult> Excluir(Guid idPublico)
    {
        var fornecedor = await _dbContext.Fornecedores
            .Where(f => f.IdPublico == idPublico)
            .Select(FornecedorMappers.ProjectToFornecedorViewModel)
            .FirstOrDefaultAsync();

        if (fornecedor is null)
        {
            _logger.LogWarning(
                "Fornecedor não encontrado ao acessar tela de exclusão. IdPublico: {IdPublico}",
                idPublico);

            return RedirectToAction(nameof(Index));
        }

        return View(fornecedor);
    }

    [HttpPost, ActionName("Excluir")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirConfirmado(Guid idPublico)
    {
        var fornecedor = await _dbContext.Fornecedores
            .Where(f => f.IdPublico == idPublico)
            .FirstOrDefaultAsync();

        if (fornecedor is null)
        {
            _logger.LogWarning(
                "Fornecedor não encontrado ao confirmar exclusão. IdPublico: {IdPublico}",
                idPublico);

            return RedirectToAction(nameof(Index));
        }

        _dbContext.Fornecedores.Remove(fornecedor);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Fornecedor excluído com sucesso. Nome: {Nome}, IdPublico: {IdPublico}, CNPJ: {Cnpj}",
            fornecedor.Nome,
            fornecedor.IdPublico,
            fornecedor.Cnpj);

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Consulta o ViaCEP e retorna o endereço formatado ou adiciona erros ao ModelState.
    /// Em caso de erro, retorna null.
    /// </summary>
    private async Task<string?> ObterEnderecoOuAdiconarErroAsync(
        string cep,
        IHttpClientFactory httpClientFactory, 
        ModelStateDictionary modelState)
    {
        try
        {
            var clienteHttp = httpClientFactory.CreateClient(ServicosExternosConstantes.ClienteViaCep);
            var cepResponse = await clienteHttp.GetFromJsonAsync<ViaCepResponse>($"{cep}/json/");

            if (cepResponse is null)
            {
                modelState.AddModelError(
                    nameof(CriarFornecedorViewModel.Cep),
                    "Não foi possível consultar o serviço de CEP.");
                return null;
            }
       
            if (cepResponse.Erro.Equals("true"))
            {
                // CEP válido com 8 dígitos mas inexistente na base do ViaCEP
                modelState.AddModelError(
                    nameof(CriarFornecedorViewModel.Cep),
                    "CEP não encontrado.");
                return null;
            }

            _logger.LogInformation("Cep {FornecedorCep} retornado: {@ViaCepResponse}", cep, cepResponse);
            string endereco = EnderecoHelpers.MontarEndereco(cepResponse);
            return endereco;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro HTTP ao consultar ViaCEP para CEP {Cep}", cep);
            modelState.AddModelError(
                nameof(CriarFornecedorViewModel.Cep),
                "Erro ao consultar o serviço de CEP. Tente novamente mais tarde.");
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error ao desserializar resposta da api ViaCEP para o CEP {Cep}", cep);
            modelState.AddModelError(
                nameof(CriarFornecedorViewModel.Cep),
                "Erro inesperado ao consultar o CEP.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar ViaCEP para CEP {Cep}", cep);
            modelState.AddModelError(
                nameof(CriarFornecedorViewModel.Cep),
                "Erro inesperado ao consultar o CEP.");
            throw;
        }
    }
}