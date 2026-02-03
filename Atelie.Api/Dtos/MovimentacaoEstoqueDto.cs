namespace Atelie.Api.Dtos
{
    public class MovimentacaoEstoqueDto
    {
        public int MaterialId { get; set; }
        public int Quantidade { get; set; }
        public string? Observacao { get; set; }
    }
}
