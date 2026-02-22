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
            string userId,
            string nomeDono,
            string email,
            string nomeAtelie,
            string periocidade)
        {
            object[] products = Array.Empty<object>();

            if (periocidade == "mensal")
            {
                products = new[]
                {
                    new
                    {
                        externalId = "pro-mensal",
                        name = "Plano Pro",
                        description = $"Assinatura Pro - {nomeAtelie}",
                        quantity = 1,
                        price = 4000
                    }
                };
            }
            else if (periocidade == "trimestral")
            {
                products = new[]
                {
                    new
                    {
                        externalId = "pro-trimestral",
                        name = "Plano Pro Trimestral",
                        description = $"Assinatura Pro Trimestral - {nomeAtelie}",
                        quantity = 1,
                        price = 10800
                    }
                };
            }
            else if (periocidade == "anual")
            {
                products = new[]
                {
                    new
                    {
                        externalId = "pro-anual",
                        name = "Plano Pro Anual",
                        description = $"Assinatura Pro Anual - {nomeAtelie}",
                        quantity = 1,
                        price = 36000
                    }
                };
            }

            var payload = new
            {
                frequency = "MULTIPLE_PAYMENTS",
                methods = new[] { "PIX" },
                products = products,
                returnUrl = "",
                completionUrl = "",
                customer = new
                {
                    name = nomeDono,
                    email = email
                },
                externalId = userId
            };

            var response = await _http.PostAsJsonAsync($"{BaseUrl}/billing/create", payload);
            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<AbacateApiResponse<BillingData>>();

            return (result!.Data.Id, result.Data.Url);
        }
    }
}