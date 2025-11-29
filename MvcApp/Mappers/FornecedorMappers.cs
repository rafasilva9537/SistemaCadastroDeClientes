using MvcApp.Models;
using MvcApp.ViewModels.Fornecedor;
using System.Linq.Expressions;
using MvcApp.ViewModels.Segmentos;

namespace MvcApp.Mappers;

/// <summary>
/// Utiliza expressions e extension methods para mapear entre Fornecedor Model e ViewModels.
/// Expression são necessárias para permitir que o Entity Framework Core converta as projeções em SQL.
/// Extension para os casos onde a projeção não é necessária, mas o mapeamento sim.
/// </summary>
public static class FornecedorMappers
{
    // Model para ViewModel
    public static Expression<Func<Fornecedor, FornecedorViewModel>> ProjectToFornecedorViewModel
        = fornecedor => new FornecedorViewModel
        {
            IdPublico = fornecedor.IdPublico,
            Nome = fornecedor.Nome,
            Cnpj = fornecedor.Cnpj,
            Cep = fornecedor.Cep,
            Endereco = fornecedor.Endereco,
            Segmento = fornecedor.Segmento.Nome,
            FotoPerfilPath = fornecedor.FotoPerfilPath
        };

    // ViewModel para Model
    public static Fornecedor ToFornecedorModel(this CriarFornecedorViewModel fornecedorViewModel)
    {
        return new Fornecedor
        {
            Nome = fornecedorViewModel.Nome,
            Cnpj = fornecedorViewModel.Cnpj,
            Cep = fornecedorViewModel.Cep,
            SegmentoId = fornecedorViewModel.SegmentoId,
        };
    }
    
    public static Expression<Func<Fornecedor, EditarFornecedorViewModel>> ProjectToEditarFornecedorViewModel
        = fornecedor => new EditarFornecedorViewModel
        {
            IdPublico = fornecedor.IdPublico,
            Nome = fornecedor.Nome,
            Cnpj = fornecedor.Cnpj,
            Cep = fornecedor.Cep,
            Endereco = fornecedor.Endereco,
            SegmentoId = fornecedor.SegmentoId
        };
    
    public static EditarFornecedorViewModel ToEditarFornecedorViewModel(this Fornecedor fornecedor, List<SegmentoViewModel> segmentos)
    {
        return new EditarFornecedorViewModel
        {
            IdPublico = fornecedor.IdPublico,
            Nome = fornecedor.Nome,
            Cnpj = fornecedor.Cnpj,
            Cep = fornecedor.Cep,
            Endereco = fornecedor.Endereco,
            SegmentoId = fornecedor.SegmentoId,
            Segmentos = segmentos
        };
    }
}
