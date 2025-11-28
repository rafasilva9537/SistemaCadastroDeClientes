using MvcApp.Models;
using MvcApp.ViewModels;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MvcApp.Mappers;

/// <summary>
/// Utiliza expressions e extension methods para mapear entre Fornecedor Model e ViewModels.
/// Expression são necessárias para permitir que o Entity Framework Core converta as projeções em SQL.
/// Extension para os casos onde a projeção não é necessária, mas o mapeamento sim.
/// </summary>
public static class FornecedoresMappers
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
            Segmento = fornecedor.Segmento!.Nome
        };


    // ViewModel para Model
    public static Fornecedor ToFornecedorModel(this CriarFornecedorViewModel fornecedorViewModel)
    {
        return new Fornecedor
        {
            Nome = fornecedorViewModel.Nome,
            Cnpj = fornecedorViewModel.Cnpj,
            Cep = fornecedorViewModel.Cep,
            SegmentoId = fornecedorViewModel.SegmentoId
        };
    }
}
