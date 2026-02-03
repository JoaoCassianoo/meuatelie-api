namespace Atelie.Api.Dtos
{
    public class EncomendaDto
    {
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorOrcado { get; set; }
        public string? Cliente { get; set; }
        public string? Observacao { get; set; }
    }
}
