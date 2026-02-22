using Microsoft.AspNetCore.Mvc;
using Atelie.Api.Services;
using Atelie.Api.Entities;
using Atelie.Api.Enums;
using Atelie.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Atelie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EncomendasController : ControllerBase
    {
        private readonly EncomendaService _service;

        public EncomendasController(EncomendaService service)
        {
            _service = service;
        }

        private Guid ObterUsuarioId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId != null ? Guid.Parse(userId) : Guid.Empty;

        }

        // POST: api/Encomendas
        [HttpPost]
        public async Task<IActionResult> CriarEncomenda([FromBody] EncomendaDto dto)
        {
            var userId = ObterUsuarioId();
            try
            {
                var encomenda = await _service.CriarEncomenda(
                    userId,
                    dto.Descricao,
                    dto.ValorOrcado,
                    dto.Cliente,
                    dto.Observacao
                );
                return CreatedAtAction(nameof(ObterPorId), new { id = encomenda.Id }, encomenda);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Encomendas
        [HttpGet]
        public async Task<IActionResult> ObterEncomendas()
        {

            var encomendas = await _service.ObterEncomendas(ObterUsuarioId());
            return Ok(encomendas);
        }

        // GET: api/Encomendas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var encomenda = await _service.ObterPorId(ObterUsuarioId(), id);
            if (encomenda == null)
                return NotFound();

            return Ok(encomenda);
        }

        // PATCH: api/Encomendas/5/status?novoStatus=2
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AtualizarStatus(int id, [FromQuery] int novoStatus)
        {
            if (!Enum.IsDefined(typeof(StatusEncomenda), novoStatus))
                return BadRequest("Status inválido");

            var sucesso = await _service.AtualizarStatus(ObterUsuarioId(), id, (StatusEncomenda)novoStatus);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Encomendas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var sucesso = await _service.Deletar(ObterUsuarioId(), id);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }
    }
}
