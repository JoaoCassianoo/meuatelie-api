using System.Security.Claims;
using Atelie.Api.Dtos;
using Atelie.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atelie.Api.Controllers
{
    [ApiController]
    [Route("api/assinatura")]
    public class AssinaturaController : ControllerBase
    {
        private readonly AssinaturaService _assinaturaService;

        public AssinaturaController(AssinaturaService assinaturaService)
        {
            _assinaturaService = assinaturaService;
        }

        [HttpPost("iniciar")]
        [Authorize]
        public async Task<IActionResult> Iniciar([FromBody] IniciarAssinaturaDto dto)
        {
            if (dto.Periodicidade.ToLower() is not ("mensal" or "trimestral" or "anual"))
                return BadRequest(new { erro = "Plano inválido. Use: mensal, trimestral ou anual." });

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var email = User.FindFirst(ClaimTypes.Email)!.Value;

            var (sucesso, erro, url) = await _assinaturaService.IniciarAssinatura(
                userId: userId,
                email: email,
                periodicidade: dto.Periodicidade
            );

            if (!sucesso)
                return BadRequest(new { erro });

            return Ok(new { url });
        }
    }
}