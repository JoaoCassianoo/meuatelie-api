using Atelie.Api.Enums;

namespace Atelie.Api.Entities
{
    public class Encomenda
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? Cliente { get; set; }
        public StatusEncomenda Status { get; set; } = StatusEncomenda.Pendente;
        public decimal ValorOrcado { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;
        public DateTime? DataFinalizacao { get; set; }
        public string? Observacao { get; set; }
    }
}
