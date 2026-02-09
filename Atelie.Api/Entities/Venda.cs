namespace Atelie.Api.Entities
{
    public class Venda
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int PecaProntaId { get; set; }
        public PecaPronta PecaPronta { get; set; } = null!;
        public int Quantidade { get; set; }
        public decimal ValorVenda { get; set; }
        public string? Cliente { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;
        public string? Observacao { get; set; }
    }
}
