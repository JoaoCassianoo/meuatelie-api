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

        
        public async Task<object> ObterPaginado(Guid userId)
        {
            var query = _context.Materiais
                .AsNoTracking()
                .Where(m => m.UserId == userId);

            var total = await query.CountAsync();

            var dados = await query
                .OrderBy(m => m.Id)
                .AsNoTracking()
                .ToListAsync();

            return dados;
        }


        public async Task<IEnumerable<Material>> ObterTodos()
        {
            return await _context.Materiais.ToListAsync();
        }

        public async Task<Material?> ObterPorId(int id)
        {
            return await _context.Materiais.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Material>> ObterPorCategoria(CategoriaMaterial categoria)
        {
            return await _context.Materiais
                .Where(m => m.Categoria == categoria)
                .ToListAsync();
        }

        public async Task<Material> Criar(Material material)
        {
            _context.Materiais.Add(material);
            await _context.SaveChangesAsync();
            return material;
        }

        public async Task<bool> Atualizar(int id, Material materialAtualizado)
        {
            var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == id);
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

        public async Task<bool> AjustarQuantidade(int id, int quantidade, TipoMovimentacaoEstoque tipo)
        {
            var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == id);
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

        public async Task<bool> Deletar(int id)
        {
            var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == id);
            if (material == null)
                return false;

            _context.Materiais.Remove(material);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> ObterQuantidadeEmEstoque(int materialId)
        {
            var material = await _context.Materiais.FirstOrDefaultAsync(m => m.Id == materialId);
            return material?.Quantidade ?? 0;
        }

        public async Task<Dtos.ResumoEstoqueDto> ObterResumoEstoque()
        {
            var materiais = await _context.Materiais.ToListAsync();

            var quantidadeTotalPecas = materiais.Sum(m => m.Quantidade);
            var valorTotalEstoque = materiais.Sum(m => m.Valor * m.Quantidade);

            return new Dtos.ResumoEstoqueDto
            {
                QuantidadeTotalPecas = quantidadeTotalPecas,
                ValorTotalEstoque = valorTotalEstoque
            };
        }
    }
}
