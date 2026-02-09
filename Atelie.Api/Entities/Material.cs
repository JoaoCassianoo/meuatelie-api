using Atelie.Api.Enums;

namespace Atelie.Api.Entities
{
    public class Material
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public CategoriaMaterial Categoria { get; set; }
        public string? Tamanho { get; set; } // Nullable para materiais genéricos
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
        public StatusMaterial Status { get; set; } = StatusMaterial.EmEstoque;
        public DateTime DataEntrada { get; set; } = DateTime.UtcNow;
        public DateTime? DataSaida { get; set; }
    }
}
