using Atelie.Api.Dtos;
using Atelie.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atelie.Api.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    [Authorize]
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
        [AllowAnonymous]
        public async Task<IActionResult> Receber(
            [FromQuery] string webhookSecret,
            [FromBody] WebhookPayload payload)
        {
            if (webhookSecret != _config["AbacatePay:WebhookSecret"])
                return Unauthorized();

            if (payload.Event == "billing.paid")
            {
                var billingId = payload.Data.Billing.Id;
                var externalIdProduto = payload.Data.Billing.Products
                    .FirstOrDefault()?.ExternalId ?? "pro-mensal";

                await _assinaturaService.AtivarAcesso(billingId, externalIdProduto);
            }

            return Ok();
        }
    }
}