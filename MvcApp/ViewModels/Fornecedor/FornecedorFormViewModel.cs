using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MvcApp.ViewModels.Segmentos;

namespace MvcApp.ViewModels.Fornecedor;

public class FornecedorFormViewModel
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cnpj é obrigatório.")]
    [RegularExpression("^[0-9]{14}$", ErrorMessage = "Cnpj deve conter 14 dígitos.")]
    public string Cnpj { get; set; } = string.Empty;

    [Required(ErrorMessage = "CEP é obrigatório.")]
    [RegularExpression("^[0-9]{8}$", ErrorMessage = "CEP deve conter 8 dígitos.")]
    public string Cep { get; set; } = string.Empty;

    [Required(ErrorMessage = "Segmento é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "Segmento inválido.")]
    public int SegmentoId { get; set; }
    
    public IFormFile? FotoPerfil { get; set; }
    
    
    /// <summary>
    /// CNPJ no formato 00.000.000/0000-00
    /// </summary>
    [BindNever]
    public string CnpjFormatado => FormataCnpj(Cnpj);

    /// <summary>
    /// CEP no formato 00000-000
    /// </summary>
    [BindNever]
    public string CepFormatado => FormataCep(Cep);

    /// <summary>
    /// Calculado pelo CEP, por isso não fazer binding
    /// </summary>
    [BindNever]
    public string Endereco { get; set; } = string.Empty;

    [BindNever]
    public List<SegmentoViewModel>? Segmentos { get; set; }
    
    
    private static string FormataCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14) return cnpj;
    
        return $"{cnpj[..2]}.{cnpj[2..5]}.{cnpj[5..8]}/{cnpj[8..12]}-{cnpj[12..14]}";
    }

    private static string FormataCep(string? cep)
    {
        if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8) return cep ?? string.Empty;

        return $"{cep[..5]}-{cep[5..8]}";
    }
}