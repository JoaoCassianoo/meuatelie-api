using Microsoft.AspNetCore.Mvc;
using Atelie.Api.Services;
using Atelie.Api.Entities;
using Atelie.Api.Enums;
using System.Security.Claims;

namespace Atelie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PecasProntasController : ControllerBase
    {
        private readonly PecaProntaService _service;

        public PecasProntasController(PecaProntaService service)
        {
            _service = service;
        }

        private Guid ObterUsuarioId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId != null ? Guid.Parse(userId) : Guid.Empty;

        }

        // POST: api/PecasProntas
        [HttpPost]
        public async Task<IActionResult> CriarPecaPronta([FromBody] CriarPecaProntaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Titulo))
                return BadRequest("Título da peça é obrigatório");

            var pecaPronta = await _service.CriarPecaPronta(
                ObterUsuarioId(),
                request.Titulo,
                request.Valor,
                request.Descricao,
                request.Tipo,
                request.FotoUrl
            );

            return CreatedAtAction(nameof(ObterPorId), new { id = pecaPronta.Id }, pecaPronta);
        }

        // GET: api/PecasProntas
        [HttpGet]
        public async Task<IActionResult> ObterTodas()
        {
            var userId = ObterUsuarioId();
            var pecasProntas = await _service.ObterTodas(userId);
            return Ok(pecasProntas);
        }

        // GET: api/PecasProntas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var userId = ObterUsuarioId();
            var pecaPronta = await _service.ObterPorId(userId, id);
            if (pecaPronta == null)
                return NotFound();

            return Ok(pecaPronta);
        }

        // PUT: api/PecasProntas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarPecaProntaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Titulo))
                return BadRequest("Título da peça é obrigatório");

            var userId = ObterUsuarioId();
            var sucesso = await _service.Atualizar(userId, id, request.Titulo, request.Valor, request.Descricao, request.FotoUrl, request.Tipo, request.Vendida);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // PUT: api/PecasProntas/5/marcar-como-vendida
        [HttpPut("{id}/marcar-como-vendida")]
        public async Task<IActionResult> MarcarComoVendida(int id)
        {
            var userId = ObterUsuarioId();
            var sucesso = await _service.MarcarComoVendida(userId, id);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // POST: api/PecasProntas/5/materiais
        [HttpPost("{id}/materiais")]
        public async Task<IActionResult> AdicionarMaterial(int id, [FromBody] AdicionarMaterialRequest request)
        {
            if (request.MaterialId <= 0 || request.QuantidadeUsada <= 0)
                return BadRequest("MaterialId e QuantidadeUsada devem ser maiores que 0");

            var userId = ObterUsuarioId();
            var sucesso = await _service.AdicionarMaterial(userId, id, request.MaterialId, request.QuantidadeUsada);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/PecasProntas/5/materiais/3
        [HttpDelete("{id}/materiais/{materialId}")]
        public async Task<IActionResult> RemoverMaterial(int id, int materialId)
        {
            var userId = ObterUsuarioId();
            var sucesso = await _service.RemoverMaterial(userId, id, materialId);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/PecasProntas/5
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

    public class CriarPecaProntaRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string? Descricao { get; set; }  // Tamanho/detalhes
        public TipoPecaPronta Tipo { get; set; }
        public string? FotoUrl { get; set; }
    }

    public class AtualizarPecaProntaRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string? Descricao { get; set; }
        public string? FotoUrl { get; set; }

        public TipoPecaPronta Tipo { get; set; }

        public bool Vendida { get; set; }
    }

    public class AdicionarMaterialRequest
    {
        public int MaterialId { get; set; }
        public int QuantidadeUsada { get; set; }
    }
}
