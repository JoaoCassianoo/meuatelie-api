using Atelie.Api.Enums;

namespace Atelie.Api.Entities
{
    public class PecaPronta
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }  // Tamanho/detalhes
        public decimal Valor { get; set; }
        public string? FotoUrl { get; set; }
        public TipoPecaPronta Tipo { get; set; }
        public bool Vendida { get; set; } = false;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Relacionamentos - Para peças a ser produzidas (já existentes podem não ter materiais definidos)
        public ICollection<PecaProntaMaterial> Materiais { get; set; } = new List<PecaProntaMaterial>();
    }
}
