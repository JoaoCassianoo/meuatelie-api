namespace Atelie.Api.Entities
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}