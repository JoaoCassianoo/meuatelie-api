using Atelie.Api.Entities;
using Atelie.Api.Services;
using Atelie.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Atelie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : ControllerBase
    {
        private readonly EstoqueService _service;

        public EstoqueController(EstoqueService service)
        {
            _service = service;
        }

        private Guid ObterUsuarioId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId != null ? Guid.Parse(userId) : Guid.Empty;

        }

        // POST: api/Estoque/entrada
        [HttpPost("entrada")]
        public async Task<IActionResult> RegistrarEntrada([FromBody] MovimentacaoEstoqueDto dto)
        {
            try
            {
                var movimentacao = await _service.RegistrarEntrada(ObterUsuarioId(), dto.MaterialId, dto.Quantidade, dto.Observacao);
                return CreatedAtAction(nameof(ObterMovimentacoes), movimentacao);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/Estoque/saida
        [HttpPost("saida")]
        public async Task<IActionResult> RegistrarSaida([FromBody] MovimentacaoEstoqueDto dto)
        {
            try
            {
                var movimentacao = await _service.RegistrarSaida(ObterUsuarioId(), dto.MaterialId, dto.Quantidade, dto.Observacao);
                return CreatedAtAction(nameof(ObterMovimentacoes), movimentacao);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Estoque/movimentacoes
        [HttpGet("movimentacoes")]
        public async Task<IActionResult> ObterMovimentacoes([FromQuery] int? materialId = null)
        {
            var userId = ObterUsuarioId();
            var movimentacoes = await _service.ObterMovimentacoes(userId, materialId);
            return Ok(movimentacoes);
        }
        
    }
}
