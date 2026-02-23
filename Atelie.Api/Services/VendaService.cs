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

        public async Task<Venda> RegistrarVenda(Guid userId, int pecaProntaId, decimal valorVenda, string? cliente = null, string? observacao = null)
        {
            var pecaPronta = await _context.PecasProntas.FirstOrDefaultAsync(p => p.Id == pecaProntaId && p.UserId == userId);
            if (pecaPronta == null)
                throw new ArgumentException("Peça pronta não encontrada");

            // Marca peça como vendida (cada peça é única)
            pecaPronta.Vendida = true;

            // Registra venda
            var venda = new Venda
            {
                UserId = userId,
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

        public async Task<IEnumerable<Venda>> ObterVendas(Guid userId, int? pecaProntaId = null)
        {
            var query = _context.Vendas
                .Where(v => v.PecaPronta.UserId == userId)
                .Include(v => v.PecaPronta)
                .AsQueryable();

            if (pecaProntaId.HasValue)
                query = query.Where(v => v.PecaProntaId == pecaProntaId.Value);

            return await query.OrderByDescending(v => v.Data).ToListAsync();
        }
        
        public async Task<Venda> Atualizar(Guid userId, int vendaId, decimal? valorVenda = null, string? cliente = null, string? observacao = null)
        {
            var venda = await _context.Vendas.FirstOrDefaultAsync(v => v.Id == vendaId && v.UserId == userId);
            if (venda == null)
                throw new ArgumentException("Venda não encontrada");

            if (valorVenda.HasValue)
                venda.ValorVenda = valorVenda.Value;

            if (cliente != null)
                venda.Cliente = cliente;

            if (observacao != null)
                venda.Observacao = observacao;

            _context.Vendas.Update(venda);
            await _context.SaveChangesAsync();

            return venda;
        }
        
        public async Task<bool> Deletar(Guid userId, int vendaId)
        {
            var venda = await _context.Vendas.FirstOrDefaultAsync(v => v.Id == vendaId && v.UserId == userId);
            if (venda == null)
                return false;

            _context.Vendas.Remove(venda);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
