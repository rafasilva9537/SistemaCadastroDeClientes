namespace MvcApp.ViewModels.Fornecedor;

public class FornecedorViewModel
{
    public Guid IdPublico { set; get; }

    public string Nome { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Cep { get; set; } = string.Empty;

    public string Endereco { get; set; } = string.Empty;

    /// <summary>
    /// Caminho relativo da foto de perfil como salvo no banco.
    /// Ex: imagens\fornecedores\xxx.jpg
    /// </summary>
    public string FotoPerfilPath { get; set; } = string.Empty;

    /// <summary>
    /// Caminho da foto pronto para ser usado em (img src="...")
    /// </summary>
    public string FotoPerfilUrl => FormatarFotoPerfilUrl(FotoPerfilPath);

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

    /// <summary>
    /// Formata caminho salvo no banco para uma URL válida
    /// troca '\' por '/'
    /// garante que começa com '/'
    /// </summary>
    private static string FormatarFotoPerfilUrl(string? caminho)
    {
        if (string.IsNullOrWhiteSpace(caminho))
            return string.Empty;

        string url = caminho.Replace('\\', '/');
        return url.StartsWith('/') ? url : "/" + url;
    }
}
