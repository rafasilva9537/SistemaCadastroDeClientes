using System.ComponentModel.DataAnnotations;

namespace MvcApp.Models;

public class Fornecedor
{
    public long Id { get; set; }

    public Guid IdPublico { set; get; }

    public string Nome { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Cep { get; set; } = string.Empty;

    public string Endereco { get; set; } = string.Empty;

    /// <summary>
    /// Endereço da foto do fornecedor, representando o caminho onde a imagem está armazenada.
    /// Ex: imagens\fornecedores\xxx.jpg
    /// </summary>
    public string FotoPerfilPath { get; set; } = string.Empty;

    public int SegmentoId { get; set; }

    public Segmento Segmento { get; set; } = null!;
}
