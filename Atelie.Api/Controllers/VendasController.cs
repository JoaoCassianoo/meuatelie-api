using Microsoft.AspNetCore.Mvc;
using Atelie.Api.Services;
using Atelie.Api.Dtos;

namespace Atelie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendasController : ControllerBase
    {
        private readonly VendaService _service;

        public VendasController(VendaService service)
        {
            _service = service;
        }

        // POST: api/Vendas
        [HttpPost]
        public async Task<IActionResult> RegistrarVenda([FromBody] VendaDto dto)
        {
            try
            {
                var venda = await _service.RegistrarVenda(
                    dto.PecaProntaId,
                    dto.ValorVenda,
                    dto.Cliente,
                    dto.Observacao
                );
                return CreatedAtAction(nameof(ObterVendas), new { id = venda.Id }, venda);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Vendas
        [HttpGet]
        public async Task<IActionResult> ObterVendas()
        {
            var vendas = await _service.ObterVendas();
            return Ok(vendas);
        }

        // GET: api/Vendas/periodo?dataInicio=2026-01-01&dataFim=2026-01-31
        [HttpGet("periodo")]
        public async Task<IActionResult> ObterVendasPorPeriodo([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            var vendas = await _service.ObterVendasPorPeriodo(dataInicio, dataFim);
            return Ok(vendas);
        }

        // GET: api/Vendas/total?dataInicio=2026-01-01&dataFim=2026-01-31
        [HttpGet("total")]
        public async Task<IActionResult> ObterTotalVendas([FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
        {
            var total = await _service.ObterTotalVendas(dataInicio, dataFim);
            return Ok(new { total });
        }

        // DELETE: api/Vendas/5
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
