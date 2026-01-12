using Atelie.Api.Data;
using Atelie.Api.Dtos;
using Atelie.Api.Enums;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Atelie.Api.Services
{
    public class FinanceiroService
    {
        private readonly AtelieDbContext _context;

        public FinanceiroService(AtelieDbContext dbContext)
        {
            _context = dbContext;   // Constructor implementation
        }

        public async Task<ResumoMensalDto> ObterResumoMensal(int ano, int mes)
        {
            var movimentacoes = await _context.MovimentacoesFinanceiro
                .Where(m => m.Data.Year == ano && m.Data.Month == mes)
                .ToListAsync();

            return new ResumoMensalDto
            {
                TotalEntradas = movimentacoes
                    .Where(m => m.Valor > 0)
                    .Sum(m => m.Valor),
                TotalSaidas = movimentacoes
                    .Where(m => m.Valor < 0)
                    .Sum(m => m.Valor),
                TotalPessoal = movimentacoes
                    .Where(m => m.Contexto == ContextoFinanceiro.Pessoal)
                    .Sum(m => m.Valor),
                TotalLoja = movimentacoes
                    .Where(m => m.Contexto == ContextoFinanceiro.Loja)
                    .Sum(m => m.Valor),
                TotalDebito = movimentacoes
                    .Where(m => m.MeioPagamento == MeioPagamento.CartaoDebito)
                    .Sum(m => m.Valor),
                TotalCredito = movimentacoes
                    .Where(m => m.MeioPagamento == MeioPagamento.CartaoCredito)
                    .Sum(m => m.Valor)
            };
        }
    }
}