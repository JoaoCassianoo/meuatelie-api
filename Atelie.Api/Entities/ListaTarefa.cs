namespace Atelie.Api.Entities
{
    public class ListaTarefa
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public ICollection<Tarefa> Tarefas { get; set; } = new List<Tarefa>();
    }
}
