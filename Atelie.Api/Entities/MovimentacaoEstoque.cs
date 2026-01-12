namespace Atelie.Api.Entities
{
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;

        public int Quantidade { get; set; }
        
        public DateTime Data { get; set; } = DateTime.Now;
        public string? Observacao { get; set; }
    }
}