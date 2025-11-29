using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MvcApp.ViewModels.Segmentos;

namespace MvcApp.ViewModels.Fornecedor;

public class CriarFornecedorViewModel
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

    /// <summary>
    /// Calculado pelo CEP, por isso não fazer binding
    /// </summary>
    [BindNever]
    public string Endereco { get; set; } = string.Empty;
    
    // public string FotoPerfilPath { get; set; } = string.Empty;

    [Required(ErrorMessage = "Segmento é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "Segmento inválido.")]
    public int SegmentoId { get; set; }
    
    [BindNever]
    public List<SegmentoViewModel>? Segmentos { get; set; }
}
