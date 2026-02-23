using Atelie.Api.Enums;

namespace Atelie.Api.Dtos
{
    public class MovimentacaoFinanceiroDto
    {
        public decimal Valor { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateTime Data { get; set; } = DateTime.UtcNow;
        public ContextoFinanceiro Contexto { get; set; } = ContextoFinanceiro.Pessoal; // Ex: Loja ou Pessoal
        public MeioPagamento MeioPagamento { get; set; } = MeioPagamento.Pix;
    }
}