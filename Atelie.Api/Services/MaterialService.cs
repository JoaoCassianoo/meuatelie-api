using Microsoft.EntityFrameworkCore;
using Atelie.Api.Data;
using Atelie.Api.Entities;
using Atelie.Api.Enums;
using Atelie.Api.Dtos;

namespace Atelie.Api.Services
{
    public class MaterialService
    {
        private readonly AtelieDbContext _context;

        public MaterialService(AtelieDbContext context)
        {
            _context = context;
        }

        
        public async Task<object> Obter(Guid userId)
        {
            var query = _context.Materiais
                .AsNoTracking()
                .Where(m => m.UserId == userId);

            var total = await query.CountAsync();

            var dados = await query
                .OrderBy(m => m.Id)
                .AsNoTracking()
                .Select(m => new MaterialDto
                {
                    Id = m.Id,
                    AtelieId = m.AtelieId,
                    Nome = m.Nome,
                    Categoria = m.Categoria,
                    Tamanho = m.Tamanho,
                    Quantidade = m.Quantidade,
                    Valor = m.Valor
                })
                .ToListAsync();

            return dados;
        }

        public async Task<Material?> ObterPorId(Guid userId, int id)
        {
            return await _context.Materiais.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        }

        public async Task<Material> Criar(Guid userId, Material material)
        {
            material.UserId = userId;
            var ultimoId = await _context.Materiais
                .Where(m => m.UserId == userId)
                .MaxAsync(m => (int?)m.AtelieId) ?? 0;
            material.AtelieId = ultimoId + 1;
            _context.Materiais.Add(material);
            await _context.SaveChangesAsync();
            return material;
        }

        public async Task<bool> Atualizar(Guid userId, int id, Material materialAtualizado)
        {
            var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (material == null)
                return false;

            material.Nome = materialAtualizado.Nome;
            material.Categoria = materialAtualizado.Categoria;
            material.Tamanho = materialAtualizado.Tamanho;
            material.Valor = materialAtualizado.Valor;
            material.Quantidade = materialAtualizado.Quantidade;

            _context.Materiais.Update(material);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AjustarQuantidade(Guid userId, int id, int quantidade, TipoMovimentacaoEstoque tipo)
        {
            var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (material == null)
                return false;

            if (tipo == TipoMovimentacaoEstoque.Entrada)
            {
                material.Quantidade += quantidade;
            }
            else if (tipo == TipoMovimentacaoEstoque.Saida)
            {
                if (material.Quantidade < quantidade)
                    return false; // Não há quantidade suficiente

                material.Quantidade -= quantidade;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Deletar(Guid userId, int id)
        {
            var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (material == null)
                return false;

            _context.Materiais.Remove(material);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> ObterQuantidadeEmEstoque(Guid userId, int materialId)
        {
            var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == materialId && m.UserId == userId);
            return material?.Quantidade ?? 0;
        }

        public async Task<ResumoEstoqueDto> ObterResumoEstoque(Guid userId)
        {

            var materiais = await _context.Materiais
                .Where(m => m.UserId == userId)
                .ToListAsync();

            var quantidadeTotalPecas = materiais.Sum(m => m.Quantidade);
            var valorTotalEstoque = materiais.Sum(m => m.Valor * m.Quantidade);

            return new ResumoEstoqueDto
            {
                QuantidadeTotalPecas = quantidadeTotalPecas,
                ValorTotalEstoque = valorTotalEstoque
            };
        }
    }
}
