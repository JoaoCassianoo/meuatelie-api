using System.Net.Http.Headers;
using Atelie.Api.Dtos;

namespace Atelie.Api.Services
{
    public class AbacatePayService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "https://api.abacatepay.com/v1";

        public AbacatePayService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", config["AbacatePay:ApiKey"]);
        }

        public async Task<(string BillingId, string BillingUrl)> CriarCobrancaAsync(
            Guid userId,
            string nomeCompleto,
            string email,
            string telefone,
            string cpfCnpj,
            string periocidade
        )
        {
            var (externalId, nome, descricao, preco) = periocidade switch
            {
                "mensal"     => ("pro-mensal",     "Plano Pro Mensal",     "Assinatura Pro Mensal",     4000),
                "trimestral" => ("pro-trimestral", "Plano Pro Trimestral", "Assinatura Pro Trimestral", 10800),
                "anual"      => ("pro-anual",      "Plano Pro Anual",      "Assinatura Pro Anual",      36000),
                _            => throw new ArgumentException($"Periodicidade inválida: {periocidade}")
            };

            var payload = new
            {
                frequency = "MULTIPLE_PAYMENTS",
                methods = new[] { "PIX" },
                products = new[]
                {
                    new
                    {
                        externalId = externalId,
                        name = nome,
                        description = descricao,
                        quantity = 1,
                        price = preco
                    }
                },
                customer = new
                {
                    name = nomeCompleto,
                    email = email,
                    cellphone = telefone,
                    taxId = cpfCnpj
                },
                externalId = userId,
                returnUrl = "https://meuatelie.vercel.app/#perfil",
                completionUrl = "https://meuatelie.vercel.app/#perfil",
            };

            var response = await _http.PostAsJsonAsync($"{BaseUrl}/billing/create", payload);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                throw new Exception($"AbacatePay erro {(int)response.StatusCode}: {erro}");
            }

            // adiciona isso antes de desserializar
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"AbacatePay resposta: {json}");

            var options = new System.Text.Json.JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            };

            var result = System.Text.Json.JsonSerializer.Deserialize<AbacateApiResponse<BillingData>>(json, options);

            if (result?.Data == null)
                throw new Exception($"AbacatePay retornou resposta inválida: {json}");

            return (result.Data.Id, result.Data.Url);
        }
    }
}