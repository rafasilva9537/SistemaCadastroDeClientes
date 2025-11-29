using MvcApp.Dtos;

namespace MvcApp.Helper;

public static class EnderecoHelpers
{
    /// <summary>
    /// Monta uma string de endereço a partir da resposta do ViaCEP.
    /// Exemplo: "Praça da Sé, lado ímpar - Sé - São Paulo/SP"
    /// </summary>
    public static string MontarEndereco(ViaCepResponse response)
    {
        // (Logradouro, Complemento) - (Bairro) - (Localidade/UF)
        var secoesDoEndereco = new List<string>(3);
   
        if (!string.IsNullOrWhiteSpace(response.Logradouro))
        {
            bool possuiComplemento = !string.IsNullOrWhiteSpace(response.Complemento);

            string logradouroFormatado;
            if (possuiComplemento)
            {
                logradouroFormatado = $"{response.Logradouro}, {response.Complemento}";
            }
            else
            {
                logradouroFormatado = response.Logradouro;
            }

            secoesDoEndereco.Add(logradouroFormatado);
        }
   
        if (!string.IsNullOrWhiteSpace(response.Bairro))
        {
            secoesDoEndereco.Add(response.Bairro);
        }
   
        var componentesLocalidade = new List<string>(2);
        if (!string.IsNullOrWhiteSpace(response.Localidade))
        {
            componentesLocalidade.Add(response.Localidade);
        }

        if (!string.IsNullOrWhiteSpace(response.Uf))
        {
            componentesLocalidade.Add(response.Uf);
        }

        if (componentesLocalidade.Count > 0)
        {
            // Evita dupla com "/" incompleto
            string localidadeFormatada = string.Join("/", componentesLocalidade);
            secoesDoEndereco.Add(localidadeFormatada);
        }

        return string.Join(" - ", secoesDoEndereco);
    }
}