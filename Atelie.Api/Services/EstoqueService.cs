using Atelie.Api.Data;
using Atelie.Api.Entities;
using Atelie.Api.Enums;
using Microsoft.EntityFrameworkCore;

namespace Atelie.Api.Services
{
    public class EstoqueService
    {
        private readonly AtelieDbContext _context;
        private readonly MaterialService _materialService;

        public EstoqueService(AtelieDbContext context, MaterialService materialService)
        {
            _context = context;
            _materialService = materialService;
        }

        public async Task<MovimentacaoEstoque> RegistrarEntrada(int materialId, int quantidade, string? observacao = null)
        {
            var material = await _materialService.ObterPorId(materialId);
            if (material == null)
                throw new ArgumentException("Material não encontrado");

            // Ajusta quantidade do material
            await _materialService.AjustarQuantidade(materialId, quantidade, TipoMovimentacaoEstoque.Entrada);

            // Registra movimentação
            var movimentacao = new MovimentacaoEstoque
            {
                MaterialId = materialId,
                Tipo = TipoMovimentacaoEstoque.Entrada,
                Quantidade = quantidade,
                Data = DateTime.UtcNow,
                Observacao = observacao
            };

            _context.MovimentacoesEstoque.Add(movimentacao);
            await _context.SaveChangesAsync();

            return movimentacao;
        }

        public async Task<MovimentacaoEstoque> RegistrarSaida(int materialId, int quantidade, string? observacao = null)
        {
            var material = await _materialService.ObterPorId(materialId);
            if (material == null)
                throw new ArgumentException("Material não encontrado");

            if (material.Quantidade < quantidade)
                throw new InvalidOperationException("Quantidade insuficiente em estoque");

            // Ajusta quantidade do material
            await _materialService.AjustarQuantidade(materialId, quantidade, TipoMovimentacaoEstoque.Saida);

            // Registra movimentação
            var movimentacao = new MovimentacaoEstoque
            {
                MaterialId = materialId,
                Tipo = TipoMovimentacaoEstoque.Saida,
                Quantidade = quantidade,
                Data = DateTime.UtcNow,
                Observacao = observacao
            };

            _context.MovimentacoesEstoque.Add(movimentacao);
            await _context.SaveChangesAsync();

            return movimentacao;
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterMovimentacoes(int? materialId = null)
        {
            var query = _context.MovimentacoesEstoque
                .Include(m => m.Material)
                .Where(m => !materialId.HasValue || m.MaterialId == materialId.Value)
                .OrderByDescending(m => m.Data);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterMovimentacoesPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.MovimentacoesEstoque
                .Include(m => m.Material)
                .Where(m => m.Data >= dataInicio && m.Data <= dataFim)
                .OrderByDescending(m => m.Data)
                .ToListAsync();
        }
    }
}
