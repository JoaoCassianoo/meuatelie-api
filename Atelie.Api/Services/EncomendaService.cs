using Microsoft.EntityFrameworkCore;
using Atelie.Api.Data;
using Atelie.Api.Entities;
using Atelie.Api.Enums;

namespace Atelie.Api.Services
{
    public class EncomendaService
    {
        private readonly AtelieDbContext _context;

        public EncomendaService(AtelieDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Encomenda>> ObterEncomendas( Guid userId)
        {
            var query = _context.Encomendas
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Data);

            return await query.ToListAsync();
        }

        public async Task<Encomenda?> ObterPorId(Guid userId, int id)
        {
            return await _context.Encomendas
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        }

        public async Task<Encomenda> CriarEncomenda(Guid userId, string descricao, decimal valorOrcado, string? cliente = null, string? observacao = null)
        {
            var encomenda = new Encomenda
            {
                UserId = userId,
                Descricao = descricao,
                Status = StatusEncomenda.Pendente,
                ValorOrcado = valorOrcado,
                Data = DateTime.UtcNow,
                Cliente = cliente,
                Observacao = observacao
            };

            _context.Encomendas.Add(encomenda);
            await _context.SaveChangesAsync();

            return encomenda;
        }

        public async Task<bool> AtualizarStatus(Guid userId, int encomendaId, StatusEncomenda novoStatus)
        {
            var encomenda = await _context.Encomendas.FirstOrDefaultAsync(e => e.Id == encomendaId && e.UserId == userId);
            if (encomenda == null)
                return false;

            encomenda.Status = novoStatus;

            if (novoStatus == StatusEncomenda.Finalizada)
            {
                encomenda.DataFinalizacao = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Deletar(Guid userId, int encomendaId)
        {
            var encomenda = await _context.Encomendas.FirstOrDefaultAsync(e => e.Id == encomendaId && e.UserId == userId);
            if (encomenda == null)
                return false;

            _context.Encomendas.Remove(encomenda);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
