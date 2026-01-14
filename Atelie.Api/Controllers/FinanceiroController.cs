using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Atelie.Api.Dtos;
using Atelie.Api.Services;
using Atelie.Api.Enums;
using Atelie.Api.Entities;
using System.Text;
using CsvHelper;
using System.Globalization;

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
        [HttpGet("resumo/mensal")]
        public async Task<IActionResult> ObterResumoMensal([FromQuery] int ano, [FromQuery] int mes)
        {
            var resumo = await _service.ObterResumoMensal(ano, mes);
            return Ok(resumo);
        }

        // GET: api/financeiro/resumo?ano=2025
        [HttpGet("resumo/anual")]
        public async Task<IActionResult> ObterResumoAnual([FromQuery] int ano)
        {
            var resumo = await _service.ObterResumoAnual(ano);
            return Ok(resumo);
        }

        // GET: api/financeiro/movimentacoes?ano=2025&mes=1
        [HttpGet("movimentacoes")]
        public async Task<IActionResult> ObterMovimentacoes(
            [FromQuery] int ano,
            [FromQuery] int mes,
            [FromQuery] ContextoFinanceiro? contexto,
            [FromQuery] MeioPagamento? meioPagamento)
        {
            var lista = await _service.ObterMovimentacoesMensais(
                ano,
                mes,
                contexto,
                meioPagamento
            );

            return Ok(lista);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, MovimentacaoFinanceiro dto)
        {
            var ok = await _service.AtualizarMovimentacao(id, dto);

            if (!ok)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var ok = await _service.ExcluirMovimentacao(id);

            if (!ok)
                return NotFound();

            return NoContent();
        }

        // POST: api/Financeiro/importar?ano=2026&mes=3
        [HttpPost("importar")]
        public async Task<IActionResult> ImportarMovimentacoes(IFormFile arquivo, [FromQuery] int? ano, [FromQuery] int? mes)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            var tempFile = Path.GetTempFileName();
            using (var stream = new FileStream(tempFile, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            try
            {
                var result = await _service.ImportarDadosCsv(tempFile, ano, mes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Retorna erro detalhado para facilitar debug do upload
                return StatusCode(500, new { message = "Erro ao importar arquivo", detail = ex.Message });
            }
        }
    }
}