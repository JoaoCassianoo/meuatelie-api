using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Atelie.Api.Dtos;
using Atelie.Api.Services;

namespace Atelie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinanceiroController: ControllerBase
    {   
        private readonly Data.AtelieDbContext _context;
        private readonly FinanceiroService _service;

        public FinanceiroController(Data.AtelieDbContext context, FinanceiroService service)
        {
            _context = context;
            _service = service;
        }

        // POST: api/Financeiro
        [HttpPost]
        public async Task<IActionResult> AdicionarMovimentacaoFinanceiro(Entities.MovimentacaoFinanceiro movimentacao)
        {
            _context.MovimentacoesFinanceiro.Add(movimentacao);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(ObterMovimentacaoFinanceiraPorId), 
                new { id = movimentacao.Id }, 
                movimentacao
            );
        }

        //GET: api/Financeiro
        [HttpGet("{id}")]
        public async Task<ActionResult> ObterMovimentacaoFinanceiraPorId(int id)
        {
            var movimentacao = await _context.MovimentacoesFinanceiro.FirstOrDefaultAsync(m => m.Id == id);
            if (movimentacao == null)
            {
                return NotFound();
            }
            return Ok(movimentacao);
        }

        // GET: api/financeiro/resumo?ano=2025&mes=1
        [HttpGet("resumo")]
        public async Task<IActionResult> ObterResumoMensal([FromQuery] int ano, [FromQuery] int mes)
        {
            var resumo = await _service.ObterResumoMensal(ano, mes);
            return Ok(resumo);
        }

    }
}