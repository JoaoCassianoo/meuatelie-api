using Microsoft.AspNetCore.Mvc;
using Atelie.Api.Services;
using Atelie.Api.Dtos;
using System.Security.Claims;

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

        private Guid ObterUsuarioId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId != null ? Guid.Parse(userId) : Guid.Empty;

        }

        // POST: api/Vendas
        [HttpPost]
        public async Task<IActionResult> RegistrarVenda([FromBody] VendaDto dto)
        {
            try
            {
                var userId = ObterUsuarioId();
                var venda = await _service.RegistrarVenda(
                    userId,
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
            var userId = ObterUsuarioId();
            var vendas = await _service.ObterVendas(userId);
            return Ok(vendas);
        }

        // DELETE: api/Vendas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var userId = ObterUsuarioId();
            var sucesso = await _service.Deletar(userId, id);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }
    }
}
