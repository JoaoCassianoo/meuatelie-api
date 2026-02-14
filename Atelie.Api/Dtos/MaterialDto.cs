using Atelie.Api.Enums;

namespace Atelie.Api.Dtos
{
    public class MaterialDto
    {
        public int Id { get; set; }
        public int AtelieId { get; set; }
        public string Nome { get; set; } = null!;
        public decimal Valor { get; set; }
        public CategoriaMaterial Categoria { get; set; }
        public string? Tamanho { get; set; } // Nullable para materiais genéricos
        public int Quantidade { get; set; }
    }
}