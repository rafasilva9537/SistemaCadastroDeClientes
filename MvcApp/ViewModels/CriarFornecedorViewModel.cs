namespace MvcApp.ViewModels;

public class CriarFornecedorViewModel
{
    public string Nome { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Cep { get; set; } = string.Empty;

    // public string FotoPerfilPath { get; set; } = string.Empty;

    public int SegmentoId { get; set; }
}
