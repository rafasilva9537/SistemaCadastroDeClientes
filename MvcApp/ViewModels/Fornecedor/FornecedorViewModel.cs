using MvcApp.Models;

namespace MvcApp.ViewModels.Fornecedor;

public class FornecedorViewModel
{
    public Guid IdPublico { set; get; }

    public string Nome { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Cep { get; set; } = string.Empty;

    public string Endereco { get; set; } = string.Empty;

    // public string FotoPerfilPath { get; set; } = string.Empty;

    public string Segmento { get; set; } = string.Empty;

    public string CnpjFormatado => FormataCnpj(Cnpj);

    public string CepFormatado => FormataCep(Cep);

    private static string FormataCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14) return cnpj;

        return $"{cnpj[..2]}.{cnpj[2..5]}.{cnpj[5..8]}/{cnpj[8..12]}-{cnpj[12..14]}";
    }

    private static string FormataCep(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8) return cep;

        return $"{cep[..5]}-{cep[5..8]}";
    }
}
