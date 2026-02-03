using System.Text.Json.Serialization;

namespace Atelie.Api.Entities
{
    public class PecaProntaMaterial
    {
        public int Id { get; set; }
        public int PecaProntaId { get; set; }

        [JsonIgnore]
        public PecaPronta PecaPronta { get; set; } = null!;
        public int MaterialId { get; set; }
        public Material Material { get; set; } = null!;
        public int QuantidadeUsada { get; set; }
    }
}
