using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Atelie.Api.Dtos;
using Atelie.Api.Services;
using Atelie.Api.Enums;
using Atelie.Api.Entities;
using System.Text;
using CsvHelper;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        private Guid ObterUsuarioId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId != null ? Guid.Parse(userId) : Guid.Empty;

        }

        // POST: api/Financeiro
        [HttpPost]
        public async Task<IActionResult> AdicionarMovimentacaoFinanceiro(MovimentacaoFinanceiro movimentacao)
        {
            var userId = ObterUsuarioId();
            var ok = await _service.AdicionarMovimentacaoFinanceiro(userId, movimentacao);
            if (!ok)
            {
                return BadRequest("Erro ao adicionar movimentação financeira.");
            }
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
            var userId = ObterUsuarioId();
            var movimentacao = await _context.MovimentacoesFinanceiro.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
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
            var userId = ObterUsuarioId();
            var resumo = await _service.ObterResumoMensal(userId, ano, mes);
            return Ok(resumo);
        }

        // GET: api/financeiro/resumo?ano=2025
        [HttpGet("resumo/anual")]
        public async Task<IActionResult> ObterResumoAnual([FromQuery] int ano)
        {
            var userId = ObterUsuarioId();
            var resumo = await _service.ObterResumoAnual(userId, ano);
            return Ok(resumo);
        }
        
        // GET: api/financeiro/movimentacoes?ano=2025&mes=1
        [HttpGet("movimentacoes")]
        public async Task<IActionResult> ObterMovimentacoes([FromQuery] int ano, [FromQuery] int mes)
        {
            var userId = ObterUsuarioId();
            var lista = await _service.ObterMovimentacoesMensais(userId, ano, mes);
            return Ok(lista);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, MovimentacaoFinanceiro dto)
        {
            var userId = ObterUsuarioId();
            var ok = await _service.AtualizarMovimentacao(userId, id, dto);

            if (!ok)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var userId = ObterUsuarioId();
            var ok = await _service.ExcluirMovimentacao(userId, id);

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