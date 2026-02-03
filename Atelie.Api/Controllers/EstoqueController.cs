using Atelie.Api.Entities;
using Atelie.Api.Services;
using Atelie.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

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

        // POST: api/Estoque/entrada
        [HttpPost("entrada")]
        public async Task<IActionResult> RegistrarEntrada([FromBody] MovimentacaoEstoqueDto dto)
        {
            try
            {
                var movimentacao = await _service.RegistrarEntrada(dto.MaterialId, dto.Quantidade, dto.Observacao);
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
                var movimentacao = await _service.RegistrarSaida(dto.MaterialId, dto.Quantidade, dto.Observacao);
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
            var movimentacoes = await _service.ObterMovimentacoes(materialId);
            return Ok(movimentacoes);
        }
        

        // GET: api/Estoque/movimentacoes/periodo?dataInicio=2026-01-01&dataFim=2026-01-31
        [HttpGet("movimentacoes/periodo")]
        public async Task<IActionResult> ObterMovimentacoesPorPeriodo([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            var movimentacoes = await _service.ObterMovimentacoesPorPeriodo(dataInicio, dataFim);
            return Ok(movimentacoes);
        }
    }
}
