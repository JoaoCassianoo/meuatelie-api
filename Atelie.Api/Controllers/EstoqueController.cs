using Atelie.Api.Entities;
using Atelie.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atelie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : ControllerBase
    {
        private readonly EstoqueService _service;

        public EstoqueController(EstoqueService service)
        {
            _service = service;
        }

        [HttpGet("produtos")]
        public async Task<IActionResult> ObterProdutos()
        {
            var produtos = await _service.ObterProdutos();
            return Ok(produtos);
        }

        [HttpPost("produtos")]
        public async Task<IActionResult> AdicionarProduto(Produto produto)
        {
            var p = await _service.AdicionarProduto(produto);
            return CreatedAtAction(nameof(ObterProdutos), new { id = p.Id }, p);
        }

        [HttpPost("movimentacoes")]
        public async Task<IActionResult> RegistrarMovimentacao(MovimentacaoEstoque movimentacao)
        {
            var m = await _service.RegistrarMovimentacao(movimentacao);
            return CreatedAtAction(nameof(ObterProdutos), new { id = m.Id }, m);
        }

        [HttpGet("estoque-atual/{produtoId}")]
        public async Task<IActionResult> ObterEstoqueAtual(int produtoId)
        {
            var qtd = await _service.ObterEstoqueAtual(produtoId);
            return Ok(new { ProdutoId = produtoId, QuantidadeAtual = qtd });
        }
    }
}
