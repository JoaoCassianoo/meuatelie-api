using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Atelie.Api.Enums;

namespace Atelie.Api.Entities
{
    public class Material
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AtelieId { get; set; }
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
