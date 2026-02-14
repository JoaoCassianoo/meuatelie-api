using Microsoft.EntityFrameworkCore;
using Atelie.Api.Data;
using Atelie.Api.Entities;

namespace Atelie.Api.Services
{
    public class TodoListService
    {
        private readonly AtelieDbContext _context;

        public TodoListService(AtelieDbContext context)
        {
            _context = context;
        }

        public async Task<ListaTarefa> CriarLista( Guid userId, string titulo)
        {
            var lista = new ListaTarefa
            {
                UserId = userId,
                Titulo = titulo,
                DataCriacao = DateTime.UtcNow
            };

            _context.ListasTarefa.Add(lista);
            await _context.SaveChangesAsync();

            return lista;
        }

        public async Task<IEnumerable<ListaTarefa>> ObterTodas(Guid userId)
        {
            return await _context.ListasTarefa
                .Where(lt => lt.UserId == userId)
                .Include(lt => lt.Tarefas)
                .OrderByDescending(lt => lt.DataCriacao)
                .ToListAsync();
        }

        public async Task<ListaTarefa?> ObterPorId(Guid userId, int id)
        {
            return await _context.ListasTarefa
                .Include(lt => lt.Tarefas)
                .FirstOrDefaultAsync(lt => lt.Id == id && lt.UserId == userId);
        }

        public async Task<Tarefa> AdicionarTarefa(Guid userId, int listaId, string descricao)
        {
            var lista = await _context.ListasTarefa.FirstOrDefaultAsync(lt => lt.Id == listaId && lt.UserId == userId);
            if (lista == null)
                throw new ArgumentException("Lista não encontrada");

            var tarefa = new Tarefa
            {
                ListaTarefaId = listaId,
                Descricao = descricao,
                Concluido = false,
                DataCriacao = DateTime.UtcNow
            };

            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            return tarefa;
        }

        public async Task<bool> ConcluirTarefa(Guid userId, int tarefaId)
        {
            var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == tarefaId && t.ListaTarefa.UserId == userId);
            if (tarefa == null)
                return false;

            tarefa.Concluido = true;
            tarefa.DataConclusao = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DesmarcaTarefa(Guid userId, int tarefaId)
        {
            var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == tarefaId && t.ListaTarefa.UserId == userId);
            if (tarefa == null)
                return false;

            tarefa.Concluido = false;
            tarefa.DataConclusao = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AtualizarTarefa(Guid userId, int tarefaId, string novaDescricao)
        {
            var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == tarefaId && t.ListaTarefa.UserId == userId);
            if (tarefa == null)
                return false;

            tarefa.Descricao = novaDescricao;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletarTarefa(Guid userId, int tarefaId)
        {
            var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == tarefaId && t.ListaTarefa.UserId == userId);
            if (tarefa == null)
                return false;

            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletarLista(Guid userId, int listaId)
        {
            var lista = await _context.ListasTarefa.FirstOrDefaultAsync(lt => lt.Id == listaId && lt.UserId == userId);
            if (lista == null)
                return false;

            _context.ListasTarefa.Remove(lista);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
