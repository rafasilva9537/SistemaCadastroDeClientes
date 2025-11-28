namespace MvcApp.ViewModels;

public class EditarFornecedorViewModel
{
    public Guid IdPublico { set; get; }

    public string Nome { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Cep { get; set; } = string.Empty;

    public string Endereco { get; set; } = string.Empty;

    // public string FotoPerfilPath { get; set; } = string.Empty;

    public string Segmento { get; set; } = string.Empty;
}
