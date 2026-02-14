using Atelie.Api.Entities;

namespace Atelie.Api.Dtos
{
    public class TodoListDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public ICollection<Tarefa> Tarefas { get; set; }
    }

    public class CriarListaDto
    {
        public string Titulo { get; set; } = string.Empty;
    }

    public class CriarTarefaDto
    {
        public string Descricao { get; set; } = string.Empty;
    }

    public class AtualizarTarefaDto
    {
        public string Descricao { get; set; } = string.Empty;
    }
}
