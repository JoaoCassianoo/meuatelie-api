using Microsoft.EntityFrameworkCore;
using Atelie.Api.Data;
using Atelie.Api.Entities;

namespace Atelie.Api.Services
{
    public class VendaService
    {
        private readonly AtelieDbContext _context;

        public VendaService(AtelieDbContext context)
        {
            _context = context;
        }

        public async Task<Venda> RegistrarVenda(int pecaProntaId, decimal valorVenda, string? cliente = null, string? observacao = null)
        {
            var pecaPronta = await _context.PecasProntas.FirstOrDefaultAsync(p => p.Id == pecaProntaId);
            if (pecaPronta == null)
                throw new ArgumentException("Peça pronta não encontrada");

            // Marca peça como vendida (cada peça é única)
            pecaPronta.Vendida = true;

            // Registra venda
            var venda = new Venda
            {
                PecaProntaId = pecaProntaId,
                Quantidade = 1,
                ValorVenda = valorVenda,
                Cliente = cliente,
                Data = DateTime.UtcNow,
                Observacao = observacao
            };

            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync();

            return venda;
        }

        public async Task<IEnumerable<Venda>> ObterVendas(int? pecaProntaId = null)
        {
            var query = _context.Vendas
                .Include(v => v.PecaPronta)
                .AsQueryable();

            if (pecaProntaId.HasValue)
                query = query.Where(v => v.PecaProntaId == pecaProntaId.Value);

            return await query.OrderByDescending(v => v.Data).ToListAsync();
        }

        public async Task<IEnumerable<Venda>> ObterVendasPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.Vendas
                .Include(v => v.PecaPronta)
                .Where(v => v.Data >= dataInicio && v.Data <= dataFim)
                .OrderByDescending(v => v.Data)
                .ToListAsync();
        }

        public async Task<decimal> ObterTotalVendas(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var query = _context.Vendas.AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(v => v.Data >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(v => v.Data <= dataFim.Value);

            return await query.SumAsync(v => v.ValorVenda);
        }

        public async Task<bool> Deletar(int vendaId)
        {
            var venda = await _context.Vendas.FirstOrDefaultAsync(v => v.Id == vendaId);
            if (venda == null)
                return false;

            _context.Vendas.Remove(venda);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
