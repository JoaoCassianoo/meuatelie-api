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

            var resultado = await _service.ObterPaginado(userId);

            return Ok(resultado);
        }


        // GET: api/Materiais
        [HttpGet("resumo")]
        public async Task<IActionResult> ObterResumoEstoque()
        {
            var resumo = await _service.ObterResumoEstoque();
            return Ok(resumo);
        }

        // GET: api/Materiais/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var material = await _service.ObterPorId(id);
            if (material == null)
                return NotFound();

            return Ok(material);
        }

        // GET: api/Materiais/categoria/1
        [HttpGet("categoria/{categoria}")]
        public async Task<IActionResult> ObterPorCategoria(int categoria)
        {
            if (!Enum.IsDefined(typeof(CategoriaMaterial), categoria))
                return BadRequest("Categoria inválida");

            var materiais = await _service.ObterPorCategoria((CategoriaMaterial)categoria);
            return Ok(materiais);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Material material)
        {
            material.UserId = ObterUsuarioId();

            var resultado = await _service.Criar(material);

            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
        }


        // PUT: api/Materiais/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Material material)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sucesso = await _service.Atualizar(id, material);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Materiais/5
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
