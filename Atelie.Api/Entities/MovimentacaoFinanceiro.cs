using Atelie.Api.Enums;

namespace Atelie.Api.Entities;

public class MovimentacaoFinanceiro
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string Descricao { get; set; } = null!;

    public decimal Valor { get; set; } // +entrada / -saida

    public ContextoFinanceiro Contexto { get; set; }
    public MeioPagamento MeioPagamento { get; set; }

    public DateTime Data { get; set; } = DateTime.Now;
}
