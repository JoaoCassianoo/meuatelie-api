namespace Atelie.Api.Dtos
{
    public class ResumoAnualDto
    {
        public decimal TotalEntradas { get; set; }
        public decimal TotalSaidas { get; set; }

        public decimal TotalLoja { get; set; }
        public decimal TotalPessoal { get; set; }

        public decimal TotalDebito { get; set; }
        public decimal TotalCredito { get; set; }
    }
}