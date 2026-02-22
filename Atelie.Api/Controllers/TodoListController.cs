using Microsoft.AspNetCore.Mvc;
using Atelie.Api.Services;
using Atelie.Api.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Atelie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TodoListController : ControllerBase
    {
        private readonly TodoListService _service;

        public TodoListController(TodoListService service)
        {
            _service = service;
        }

        private Guid ObterUsuarioId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId != null ? Guid.Parse(userId) : Guid.Empty;

        }

        // POST: api/TodoList
        [HttpPost]
        public async Task<IActionResult> CriarLista([FromBody] CriarListaDto dto)
        {
            var userId = ObterUsuarioId();
            var lista = await _service.CriarLista(userId, dto.Titulo);
            return CreatedAtAction(nameof(ObterPorId), new { id = lista.Id }, lista);
        }

        // GET: api/TodoList
        [HttpGet]
        public async Task<IActionResult> ObterTodas()
        {
            var userId = ObterUsuarioId();
            var listas = await _service.ObterTodas(userId);
            return Ok(listas);
        }

        // GET: api/TodoList/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var userId = ObterUsuarioId();
            var lista = await _service.ObterPorId(userId, id);
            if (lista == null)
                return NotFound();

            return Ok(lista);
        }

        // POST: api/TodoList/5/tarefas
        [HttpPost("{listaId}/tarefas")]
        public async Task<IActionResult> AdicionarTarefa(int listaId, [FromBody] CriarTarefaDto dto)
        {
            try
            {
                var userId = ObterUsuarioId();
                var tarefa = await _service.AdicionarTarefa(userId, listaId, dto.Descricao);
                return CreatedAtAction(nameof(ObterPorId), new { id = listaId }, tarefa);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH: api/TodoList/tarefas/5/concluir
        [HttpPatch("tarefas/{tarefaId}/concluir")]
        public async Task<IActionResult> ConcluirTarefa(int tarefaId)
        {
            var userId = ObterUsuarioId();
            var sucesso = await _service.ConcluirTarefa(userId, tarefaId);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // PATCH: api/TodoList/tarefas/5/desmarcar
        [HttpPatch("tarefas/{tarefaId}/desmarcar")]
        public async Task<IActionResult> DesmarcaTarefa(int tarefaId)
        {
            var userId = ObterUsuarioId();
            var sucesso = await _service.DesmarcaTarefa(userId, tarefaId);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // PUT: api/TodoList/tarefas/5
        [HttpPut("tarefas/{tarefaId}")]
        public async Task<IActionResult> AtualizarTarefa(int tarefaId, [FromBody] AtualizarTarefaDto dto)
        {
            var userId = ObterUsuarioId();
            var sucesso = await _service.AtualizarTarefa(userId, tarefaId, dto.Descricao);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/TodoList/tarefas/5
        [HttpDelete("tarefas/{tarefaId}")]
        public async Task<IActionResult> DeletarTarefa(int tarefaId)
        {
            var userId = ObterUsuarioId();
            var sucesso = await _service.DeletarTarefa(userId, tarefaId);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/TodoList/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarLista(int id)
        {
            var userId = ObterUsuarioId();
            var sucesso = await _service.DeletarLista(userId, id);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }
    }
}
