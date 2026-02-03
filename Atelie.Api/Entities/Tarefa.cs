using System.Text.Json.Serialization;

namespace Atelie.Api.Entities
{
    public class Tarefa
    {
        public int Id { get; set; }
        public int ListaTarefaId { get; set; }
        [JsonIgnore]
        public ListaTarefa ListaTarefa { get; set; } = null!;
        public string Descricao { get; set; } = string.Empty;
        public bool Concluido { get; set; } = false;
        public DateTime? DataConclusao { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}
