using Microsoft.EntityFrameworkCore;
using Atelie.Api.Data;
using Atelie.Api.Entities;
using Atelie.Api.Enums;

namespace Atelie.Api.Services
{
    public class PecaProntaService
    {
        private readonly AtelieDbContext _context;

        public PecaProntaService(AtelieDbContext context)
        {
            _context = context;
        }

        public async Task<PecaPronta> CriarPecaPronta(Guid userId, string titulo, decimal valor, string? descricao = null, TipoPecaPronta tipo = TipoPecaPronta.Produzida, string? fotoUrl = null)
        {
            var pecaPronta = new PecaPronta
            {
                UserId = userId,
                Titulo = titulo,
                Valor = valor,
                Descricao = descricao,
                Tipo = tipo,
                FotoUrl = fotoUrl,
                Vendida = false,
                DataCriacao = DateTime.UtcNow
            };

            _context.PecasProntas.Add(pecaPronta);
            await _context.SaveChangesAsync();

            return pecaPronta;
        }

        public async Task<IEnumerable<PecaPronta>> ObterTodas(Guid userId)
        {
            return await _context.PecasProntas
                .Include(p => p.Materiais)
                .ThenInclude(pm => pm.Material)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();
        }

        public async Task<PecaPronta?> ObterPorId(Guid userId, int id)
        {
            return await _context.PecasProntas
                .Include(p => p.Materiais)
                .ThenInclude(pm => pm.Material)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        }

        public async Task<bool> Atualizar(Guid userId, int id, string titulo, decimal valor, string? descricao = null, string? fotoUrl = null, TipoPecaPronta? tipo = null, bool? vendida = null)
        {
            var pecaPronta = await _context.PecasProntas.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (pecaPronta == null)
                return false;

            pecaPronta.Titulo = titulo;
            pecaPronta.Valor = valor;
            pecaPronta.Descricao = descricao;
            pecaPronta.Tipo = (TipoPecaPronta)tipo;
            pecaPronta.FotoUrl = fotoUrl;
            pecaPronta.Vendida = (bool)vendida;

            _context.PecasProntas.Update(pecaPronta);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarcarComoVendida(Guid userId, int id)
        {
            var pecaPronta = await _context.PecasProntas.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (pecaPronta == null)
                return false;

            pecaPronta.Vendida = true;

            _context.PecasProntas.Update(pecaPronta);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdicionarMaterial(Guid userId, int pecaProntaId, int materialId, int quantidadeUsada)
        {
            var pecaPronta = await _context.PecasProntas.FirstOrDefaultAsync(p => p.Id == pecaProntaId && p.UserId == userId);
            var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == materialId && m.UserId == userId);

            if (pecaPronta == null || material == null)
                return false;

            var pecaProntaMaterial = new PecaProntaMaterial
            {
                PecaProntaId = pecaProntaId,
                MaterialId = materialId,
                QuantidadeUsada = quantidadeUsada
            };

            _context.PecaProntaMateriais.Add(pecaProntaMaterial);

            // Diminui a quantidade do material (se for peça a produzir)
            if (pecaPronta.Tipo == TipoPecaPronta.Produzida)
            {
                material.Quantidade -= quantidadeUsada;
                _context.Materiais.Update(material);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoverMaterial(Guid userId, int pecaProntaId, int materialId)
        {
            var pecaProntaMaterial = await _context.PecaProntaMateriais
                .Include(p => p.PecaPronta)
                .FirstOrDefaultAsync(pm => pm.PecaProntaId == pecaProntaId && pm.MaterialId == materialId && pm.PecaPronta.UserId == userId);

            if (pecaProntaMaterial == null)
                return false;

            // Devolve a quantidade do material (se for peça a produzir)
            if (pecaProntaMaterial.PecaPronta.Tipo == TipoPecaPronta.Produzida)
            {
                var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == materialId && m.UserId == userId);
                if (material != null)
                {
                    material.Quantidade += pecaProntaMaterial.QuantidadeUsada;
                    _context.Materiais.Update(material);
                }
            }

            _context.PecaProntaMateriais.Remove(pecaProntaMaterial);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Deletar(Guid userId, int id)
        {
            var pecaPronta = await _context.PecasProntas.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (pecaPronta == null)
                return false;

            // Devolve os materiais ao estoque (se for peça a produzir)
            if (pecaPronta.Tipo == TipoPecaPronta.Produzida)
            {
                var materiais = await _context.PecaProntaMateriais
                    .Where(pm => pm.PecaProntaId == id)
                    .ToListAsync();

                foreach (var pm in materiais)
                {
                    var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == pm.MaterialId && m.UserId == userId);
                    if (material != null)
                    {
                        material.Quantidade += pm.QuantidadeUsada;
                        _context.Materiais.Update(material);
                    }
                }
            }

            _context.PecasProntas.Remove(pecaPronta);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
