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

        public async Task<PecaPronta> CriarPecaPronta(string titulo, decimal valor, string? descricao = null, TipoPecaPronta tipo = TipoPecaPronta.JaExistente, string? fotoUrl = null)
        {
            var pecaPronta = new PecaPronta
            {
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

        public async Task<IEnumerable<PecaPronta>> ObterTodas()
        {
            return await _context.PecasProntas
                .Include(p => p.Materiais)
                .ThenInclude(pm => pm.Material)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<PecaPronta>> ObterPecasNaoVendidas()
        {
            return await _context.PecasProntas
                .Include(p => p.Materiais)
                .ThenInclude(pm => pm.Material)
                .Where(p => !p.Vendida)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<PecaPronta>> ObterPorTipo(TipoPecaPronta tipo)
        {
            return await _context.PecasProntas
                .Include(p => p.Materiais)
                .ThenInclude(pm => pm.Material)
                .Where(p => p.Tipo == tipo)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();
        }

        public async Task<PecaPronta?> ObterPorId(int id)
        {
            return await _context.PecasProntas
                .Include(p => p.Materiais)
                .ThenInclude(pm => pm.Material)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> Atualizar(int id, string titulo, decimal valor, string? descricao = null, string? fotoUrl = null, TipoPecaPronta? tipo = null, bool? vendida = null)
        {
            var pecaPronta = await _context.PecasProntas.FirstOrDefaultAsync(p => p.Id == id);
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

        public async Task<bool> MarcarComoVendida(int id)
        {
            var pecaPronta = await _context.PecasProntas.FirstOrDefaultAsync(p => p.Id == id);
            if (pecaPronta == null)
                return false;

            pecaPronta.Vendida = true;

            _context.PecasProntas.Update(pecaPronta);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdicionarMaterial(int pecaProntaId, int materialId, int quantidadeUsada)
        {
            var pecaPronta = await _context.PecasProntas.FirstOrDefaultAsync(p => p.Id == pecaProntaId);
            var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == materialId);

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

        public async Task<bool> RemoverMaterial(int pecaProntaId, int materialId)
        {
            var pecaProntaMaterial = await _context.PecaProntaMateriais
                .Include(p => p.PecaPronta)
                .FirstOrDefaultAsync(pm => pm.PecaProntaId == pecaProntaId && pm.MaterialId == materialId);

            if (pecaProntaMaterial == null)
                return false;

            // Devolve a quantidade do material (se for peça a produzir)
            if (pecaProntaMaterial.PecaPronta.Tipo == TipoPecaPronta.Produzida)
            {
                var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == materialId);
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

        public async Task<bool> Deletar(int id)
        {
            var pecaPronta = await _context.PecasProntas.FirstOrDefaultAsync(p => p.Id == id);
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
                    var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == pm.MaterialId);
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
