using System.Collections.Immutable;

namespace MvcApp.Services;

/// <summary>
/// Serviço para armazenamento de imagens que abstrai o tipo de armazenamento.
/// Podendo ser usado, por exemplo, localmente, em cloud, etc.
/// </summary>
public interface IImagemService
{
    /// <summary>
    /// Retorna o caminho relativo da imagem salva.
    /// </summary>
    Task<string> SalvarImagemAsync(IFormFile imagem, string categoria);
    FileStream ObterImagem(string nomeImagem, string categoria);
}

/// <summary>
/// Implementação de IImagemService que salva as imagens em disco local
/// na pasta wwwroot/imagens/{categoria}.
/// </summary>
public class ImagemLocalService : IImagemService
{
    public static readonly ImmutableArray<string> ExtensoesValidas = [".jpg", ".png", ".jpeg"];
    private const string PastaImagens = "imagens";
    private readonly IWebHostEnvironment _environment;

    public ImagemLocalService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SalvarImagemAsync(IFormFile imagem, string categoria)
    {
        if (imagem.Length <= 0)
        {
            throw new ArgumentException("Nenhuma imagem foi enviada.", nameof(imagem));
        }

        if (string.IsNullOrWhiteSpace(categoria))
        {
            categoria = "geral";
        }

        string raizWeb = _environment.WebRootPath;
        string caminhoPastaImagens = Path.Combine(raizWeb, PastaImagens, categoria);

        if (!Directory.Exists(caminhoPastaImagens))
        {
            Directory.CreateDirectory(caminhoPastaImagens);
        }

        string nomeArquivoOriginal = imagem.FileName;
        string extensao = Path.GetExtension(nomeArquivoOriginal);

        if (!ExtensoesValidas.Contains(extensao, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentOutOfRangeException(nameof(imagem),
                $"Apenas arquivos com as extensões {string.Join(", ", ExtensoesValidas)} são permitidos.");
        }

        string nomeImagem = $"{Guid.NewGuid()}{extensao}";
        string caminhoCompleto = Path.Combine(caminhoPastaImagens, nomeImagem);

        await using FileStream imagemStream = new(caminhoCompleto, FileMode.CreateNew);
        await imagem.CopyToAsync(imagemStream);
        
        return Path.Combine(categoria, nomeImagem);
    }

    public FileStream ObterImagem(string nomeImagem, string categoria)
    {
        if (string.IsNullOrWhiteSpace(nomeImagem) && string.IsNullOrWhiteSpace(categoria))
        {
            throw new ArgumentException("Nome da imagem e categoria devem ser fornecidos.");
        }

        string raizWeb = _environment.WebRootPath;
        string caminhoPastaImagens = Path.Combine(raizWeb, PastaImagens, categoria);
        string caminhoCompleto = Path.Combine(caminhoPastaImagens, nomeImagem);

        if (!File.Exists(caminhoCompleto))
        {
            throw new FileNotFoundException("Imagem não encontrada.", caminhoCompleto);
        }

        var imagemStream = File.OpenRead(caminhoCompleto);
        return imagemStream;
    }
}