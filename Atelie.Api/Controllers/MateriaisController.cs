using Microsoft.AspNetCore.Mvc;
using Atelie.Api.Services;
using Atelie.Api.Entities;
using Atelie.Api.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Atelie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MateriaisController : ControllerBase
    {
        private readonly MaterialService _service;

        public MateriaisController(MaterialService service)
        {
            _service = service;
        }

        private Guid ObterUsuarioId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId != null ? Guid.Parse(userId) : Guid.Empty;

        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var userId = ObterUsuarioId();

            var resultado = await _service.Obter(userId);

            return Ok(resultado);
        }

        // GET: api/Materiais
        [HttpGet("resumo")]
        public async Task<IActionResult> ObterResumoEstoque()
        {
            var userId = ObterUsuarioId();
            var resumo = await _service.ObterResumoEstoque(userId);
            return Ok(resumo);
        }

        // GET: api/Materiais/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var userId = ObterUsuarioId();
            var material = await _service.ObterPorId(userId, id);
            if (material == null)
                return NotFound();

            return Ok(material);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Material material)
        {
            material.UserId = ObterUsuarioId();

            var resultado = await _service.Criar(ObterUsuarioId(), material);

            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
        }

        // PUT: api/Materiais/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Material material)
        {
            var userId = ObterUsuarioId();
            material.UserId = userId;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sucesso = await _service.Atualizar(userId, id, material);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Materiais/5
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
