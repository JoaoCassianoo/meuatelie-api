namespace Atelie.Api.Dtos
{
    public class VendaDto
    {
        public int PecaProntaId { get; set; }
        public decimal ValorVenda { get; set; }
        public string? Cliente { get; set; }
        public string? Observacao { get; set; }
        public DateTime Data { get; set; } 
    }
}
