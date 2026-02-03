using Microsoft.AspNetCore.Mvc;
using Atelie.Api.Services;
using Atelie.Api.Entities;
using Atelie.Api.Enums;
using Atelie.Api.Dtos;

namespace Atelie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EncomendasController : ControllerBase
    {
        private readonly EncomendaService _service;

        public EncomendasController(EncomendaService service)
        {
            _service = service;
        }

        // POST: api/Encomendas
        [HttpPost]
        public async Task<IActionResult> CriarEncomenda([FromBody] EncomendaDto dto)
        {
            try
            {
                var encomenda = await _service.CriarEncomenda(
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
        public async Task<IActionResult> ObterEncomendas([FromQuery] int? status = null)
        {
            StatusEncomenda? statusEnum = null;
            if (status.HasValue && Enum.IsDefined(typeof(StatusEncomenda), status.Value))
                statusEnum = (StatusEncomenda)status.Value;

            var encomendas = await _service.ObterEncomendas(statusEnum);
            return Ok(encomendas);
        }

        // GET: api/Encomendas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var encomenda = await _service.ObterPorId(id);
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

            var sucesso = await _service.AtualizarStatus(id, (StatusEncomenda)novoStatus);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Encomendas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var sucesso = await _service.Deletar(id);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }
    }
}
