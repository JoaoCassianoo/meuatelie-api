using Atelie.Api.Enums;

namespace Atelie.Api.Entities
{
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int MaterialId { get; set; }
        public Material Material { get; set; } = null!;
        
        public int Quantidade { get; set; } // sempre positivo
        public TipoMovimentacaoEstoque Tipo { get; set; } // Entrada ou Saída
        public DateTime Data { get; set; } = DateTime.UtcNow;

        public string? Observacao { get; set; }
    }
}
