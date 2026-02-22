using Atelie.Api.Dtos;
using Atelie.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atelie.Api.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly AssinaturaService _assinaturaService;
        private readonly IConfiguration _config;

        public WebhookController(AssinaturaService assinaturaService, IConfiguration config)
        {
            _assinaturaService = assinaturaService;
            _config = config;
        }

        [HttpPost("pagamento")]
        public async Task<IActionResult> Receber(
            [FromQuery] string secret,
            [FromBody] WebhookPayload payload)
        {
            if (secret != _config["AbacatePay:WebhookSecret"])
                return Unauthorized();

            Console.WriteLine(payload.Data.Billing.ExternalId);

            if (payload.Event == "billing.paid")
            {
                var userId = payload.Data.Billing.ExternalId;
                var billingId = payload.Data.Billing.Id;
                var externalIdProduto = payload.Data.Billing.Products
                    .FirstOrDefault()?.ExternalId ?? "pro-mensal";

                await _assinaturaService.AtivarAcesso(userId, billingId, externalIdProduto);
            }

            return Ok();
        }
    }
}