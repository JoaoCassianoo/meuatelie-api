using Microsoft.AspNetCore.Mvc;
using Atelie.Api.Services;
using Atelie.Api.Dtos;

namespace Atelie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoListController : ControllerBase
    {
        private readonly TodoListService _service;

        public TodoListController(TodoListService service)
        {
            _service = service;
        }

        // POST: api/TodoList
        [HttpPost]
        public async Task<IActionResult> CriarLista([FromBody] CriarListaDto dto)
        {
            var lista = await _service.CriarLista(dto.Titulo);
            return CreatedAtAction(nameof(ObterPorId), new { id = lista.Id }, lista);
        }

        // GET: api/TodoList
        [HttpGet]
        public async Task<IActionResult> ObterTodas()
        {
            var listas = await _service.ObterTodas();
            return Ok(listas);
        }

        // GET: api/TodoList/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var lista = await _service.ObterPorId(id);
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
                var tarefa = await _service.AdicionarTarefa(listaId, dto.Descricao);
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
            var sucesso = await _service.ConcluirTarefa(tarefaId);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // PATCH: api/TodoList/tarefas/5/desmarcar
        [HttpPatch("tarefas/{tarefaId}/desmarcar")]
        public async Task<IActionResult> DesmarcaTarefa(int tarefaId)
        {
            var sucesso = await _service.DesmarcaTarefa(tarefaId);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // PUT: api/TodoList/tarefas/5
        [HttpPut("tarefas/{tarefaId}")]
        public async Task<IActionResult> AtualizarTarefa(int tarefaId, [FromBody] AtualizarTarefaDto dto)
        {
            var sucesso = await _service.AtualizarTarefa(tarefaId, dto.Descricao);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/TodoList/tarefas/5
        [HttpDelete("tarefas/{tarefaId}")]
        public async Task<IActionResult> DeletarTarefa(int tarefaId)
        {
            var sucesso = await _service.DeletarTarefa(tarefaId);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/TodoList/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarLista(int id)
        {
            var sucesso = await _service.DeletarLista(id);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }
    }
}
