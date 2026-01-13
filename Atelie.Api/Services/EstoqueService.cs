using Atelie.Api.Data;
using Atelie.Api.Entities;
using Atelie.Api.Enums;
using Microsoft.EntityFrameworkCore;

namespace Atelie.Api.Services
{
    public class EstoqueService
    {
        private readonly AtelieDbContext _context;

        public EstoqueService(AtelieDbContext context)
        {
            _context = context;
        }

        public async Task<List<Produto>> ObterProdutos()
        {
            return await _context.Produtos
                .Include(p => p.Movimentacoes)
                .ToListAsync();
        }

        public async Task<Produto> AdicionarProduto(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<MovimentacaoEstoque> RegistrarMovimentacao(MovimentacaoEstoque movimentacao)
        {
            _context.MovimentacoesEstoque.Add(movimentacao);
            await _context.SaveChangesAsync();
            return movimentacao;
        }

        public async Task<int> ObterEstoqueAtual(int produtoId)
        {
            var entradas = await _context.MovimentacoesEstoque
                .Where(m => m.ProdutoId == produtoId && m.Tipo == TipoMovimentacao.Entrada)
                .SumAsync(m => m.Quantidade);

            var saidas = await _context.MovimentacoesEstoque
                .Where(m => m.ProdutoId == produtoId && m.Tipo == TipoMovimentacao.Saida)
                .SumAsync(m => m.Quantidade);

            return entradas - saidas;
        }
    }
}
