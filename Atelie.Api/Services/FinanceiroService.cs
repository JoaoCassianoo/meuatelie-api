using Atelie.Api.Data;
using Atelie.Api.Dtos;
using Atelie.Api.Entities;
using Atelie.Api.Enums;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Atelie.Api.Services
{
    public class FinanceiroService
    {
        private readonly AtelieDbContext _context;

        public FinanceiroService(AtelieDbContext context)
        {
            _context = context;
        }

        #region Resumos

        public async Task<ResumoMensalDto> ObterResumoMensal(int ano, int mes)
        {
            var movs = await _context.MovimentacoesFinanceiro
                .Where(m => m.Data.Year == ano && m.Data.Month == mes)
                .ToListAsync();

            return new ResumoMensalDto
            {
                TotalEntradas = movs.Where(x => x.Valor > 0).Sum(x => x.Valor),
                TotalSaidas = movs.Where(x => x.Valor < 0).Sum(x => x.Valor),
                TotalPessoal = movs.Sum(x => x.Valor),
                TotalEntradasPessoal = movs.Where(x => x.Valor > 0).Sum(x => x.Valor),
                TotalSaidasPessoal = movs.Where(x => x.Valor < 0).Sum(x => x.Valor),
                TotalEntradasLoja = 0,
                TotalSaidasLoja = 0,
                TotalLoja = 0,
                TotalDebito = movs.Where(x => x.MeioPagamento == MeioPagamento.CartaoDebito).Sum(x => x.Valor),
                TotalCredito = movs.Where(x => x.MeioPagamento == MeioPagamento.CartaoCredito).Sum(x => x.Valor)
            };
        }

        public async Task<ResumoAnualDto> ObterResumoAnual(int ano)
        {
            var movs = await _context.MovimentacoesFinanceiro
                .Where(m => m.Data.Year == ano)
                .ToListAsync();

            return new ResumoAnualDto
            {
                TotalEntradas = movs.Where(x => x.Valor > 0).Sum(x => x.Valor),
                TotalSaidas = movs.Where(x => x.Valor < 0).Sum(x => x.Valor),
                TotalPessoal = movs.Sum(x => x.Valor),
                TotalLoja = 0,
                TotalDebito = movs.Where(x => x.MeioPagamento == MeioPagamento.CartaoDebito).Sum(x => x.Valor),
                TotalCredito = movs.Where(x => x.MeioPagamento == MeioPagamento.CartaoCredito).Sum(x => x.Valor)
            };
        }

        #endregion

        #region CRUD

        public async Task<List<MovimentacaoFinanceiro>> ObterMovimentacoesMensais(
            int ano,
            int mes,
            int? tipo,
            ContextoFinanceiro? contexto,
            MeioPagamento? meioPagamento)
        {
            var query = _context.MovimentacoesFinanceiro
                .Where(m => m.Data.Year == ano && m.Data.Month == mes);

            if (meioPagamento.HasValue)
                query = query.Where(m => m.MeioPagamento == meioPagamento.Value);

            if (tipo == 1)
                query = query.Where(m => m.Valor > 0);
            else if (tipo == 2)
                query = query.Where(m => m.Valor < 0);

            return await query
                .OrderByDescending(m => m.Data)
                .ToListAsync();
        }

        public async Task<bool> AtualizarMovimentacao(int id, MovimentacaoFinanceiro dto)
        {
            var mov = await _context.MovimentacoesFinanceiro.FindAsync(id);
            if (mov == null) return false;

            mov.Descricao = CleanAndExtractName(dto.Descricao);
            mov.Valor = dto.Valor;
            mov.Contexto = dto.Contexto;
            mov.MeioPagamento = dto.MeioPagamento;
            mov.Data = dto.Data;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExcluirMovimentacao(int id)
        {
            var mov = await _context.MovimentacoesFinanceiro.FindAsync(id);
            if (mov == null) return false;

            _context.MovimentacoesFinanceiro.Remove(mov);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Importação CSV

        public async Task<ImportResultDto> ImportarDadosCsv(string caminhoArquivo, int? ano = null, int? mes = null)
        {
            var result = new ImportResultDto();
            var ptBr = CultureInfo.GetCultureInfo("pt-BR");

            var text = await File.ReadAllTextAsync(caminhoArquivo, Encoding.UTF8);
            var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            int targetMonth = mes ?? DetectMonthFromFileName(caminhoArquivo) ?? DateTime.Now.Month;
            int targetYear = ano ?? DateTime.Now.Year;

            int imported = 0;

            var firstLine = lines[0].ToLowerInvariant();
            var titles = firstLine.Split(',');
            var isCartao = true;
            if(titles.Length > 3)
                isCartao = false;

            if (isCartao)
            {
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split(',');

                    DateTime data;
                    var dataStr = parts[0].Trim();
                    if (int.TryParse(dataStr, out var day))
                        data = new DateTime(targetYear, targetMonth, Math.Clamp(day, 1, 28));
                    else if (!DateTime.TryParse(dataStr, ptBr, DateTimeStyles.None, out data))
                        data = DateTime.Now;

                    var descricao = parts[1].Trim();
                    var meio = DetectarMeioPagamento(parts[1].Trim(), isCartao);
                    
                    var valor = parts[2];
                    var valorNormalizado = valor.Replace(".", ",");
                    var valorFinal = -decimal.Parse(valorNormalizado, ptBr);

                    Console.WriteLine($"Parsed data: {data}, descricao: {descricao}, valor: {valorFinal}, meio: {meio}");

                    _context.MovimentacoesFinanceiro.Add(new MovimentacaoFinanceiro
                    {
                        Descricao = descricao,
                        Valor = valorFinal,
                        Contexto = ContextoFinanceiro.Pessoal,
                        MeioPagamento = meio,
                        Data = data
                    });

                    imported++;
                }
            }
            else if(!isCartao){

                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var lineNumber = i;

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split(',');
                    
                    Console.WriteLine($"{i} Split into {parts.Length} parts");

                    if (parts.Length < 3)
                    {
                        Console.WriteLine($"Skipping line {lineNumber}: not enough parts");
                        continue;
                    }

                    DateTime data;
                    var dataStr = parts[0].Trim();
                    if (int.TryParse(dataStr, out var day))
                        data = new DateTime(targetYear, targetMonth, Math.Clamp(day, 1, 28));
                    else if (!DateTime.TryParse(dataStr, ptBr, DateTimeStyles.None, out data))
                        data = DateTime.Now;

                    var descricao = CleanAndExtractName(parts[3].Trim());
                    var meio = DetectarMeioPagamento(parts[3].Trim(), isCartao);
                    
                    var valor = parts[1];
                    var valorNormalizado = valor.Replace(".", ",");


                    Console.WriteLine($"Parsed data: {data}, descricao: {descricao}, valor: {valor}, meio: {meio}");

                    _context.MovimentacoesFinanceiro.Add(new MovimentacaoFinanceiro
                    {
                        Descricao = descricao,
                        Valor = decimal.Parse(valorNormalizado, ptBr),
                        Contexto = ContextoFinanceiro.Pessoal,
                        MeioPagamento = meio,
                        Data = data
                    });

                    imported++;
                }
            }

            await _context.SaveChangesAsync();
            result.Imported = imported;
            return result;
        }

        private static MeioPagamento DetectarMeioPagamento(string descricao, bool isCartao)
        {
            var d = descricao.ToLowerInvariant();

            if (isCartao)
                return MeioPagamento.CartaoCredito;

            if (d.Contains("débito") || d.Contains("debito"))
                return MeioPagamento.CartaoDebito;

            if (d.Contains("crédito") || d.Contains("credito"))
                return MeioPagamento.CartaoCredito;

            if (d.Contains("pix"))
                return MeioPagamento.Pix;

            return MeioPagamento.Pix;
        }

        private static string CleanAndExtractName(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                return string.Empty;

            var d = descricao.ToUpperInvariant();

            if (!d.Contains('-'))
                return d.Trim();

            var parts = d.Split('-');

            return parts[1].Trim();

        }

        private static int? DetectMonthFromFileName(string path)
        {
            var name = Path.GetFileName(path)?.ToUpperInvariant() ?? "";

            var months = new Dictionary<string, int>
            {
                {"JANEIRO",1},{"FEVEREIRO",2},{"MARCO",3},{"MARÇO",3},{"ABRIL",4},
                {"MAIO",5},{"JUNHO",6},{"JULHO",7},{"AGOSTO",8},
                {"SETEMBRO",9},{"OUTUBRO",10},{"NOVEMBRO",11},{"DEZEMBRO",12}
            };

            foreach (var m in months)
                if (name.Contains(m.Key))
                    return m.Value;

            return null;
        }

        #endregion
    }
}
