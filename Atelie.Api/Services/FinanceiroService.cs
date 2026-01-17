using Atelie.Api.Data;
using Atelie.Api.Dtos;
using Atelie.Api.Entities;
using Atelie.Api.Enums;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace Atelie.Api.Services
{
    public class FinanceiroService
    {
        private readonly AtelieDbContext _context;

        public FinanceiroService(AtelieDbContext dbContext)
        {
            _context = dbContext;   // Constructor implementation
        }

        public async Task<ResumoMensalDto> ObterResumoMensal(int ano, int mes)
        {
            var movimentacoes = await _context.MovimentacoesFinanceiro
                .Where(m => m.Data.Year == ano && m.Data.Month == mes)
                .ToListAsync();

            return new ResumoMensalDto
            {
                TotalEntradas = movimentacoes
                    .Where(m => m.Valor > 0)
                    .Sum(m => m.Valor),
                TotalSaidas = movimentacoes
                    .Where(m => m.Valor < 0)
                    .Sum(m => m.Valor),
                TotalPessoal = movimentacoes
                    .Where(m => m.Contexto == ContextoFinanceiro.Pessoal)
                    .Sum(m => m.Valor),
                TotalEntradasPessoal = movimentacoes
                    .Where(m => m.Contexto == ContextoFinanceiro.Pessoal && m.Valor > 0)
                    .Sum(m => m.Valor),
                TotalSaidasPessoal = movimentacoes
                    .Where(m => m.Contexto == ContextoFinanceiro.Pessoal && m.Valor < 0)
                    .Sum(m => m.Valor),
                TotalEntradasLoja = movimentacoes
                    .Where(m => m.Contexto == ContextoFinanceiro.Loja && m.Valor > 0)
                    .Sum(m => m.Valor),
                TotalSaidasLoja = movimentacoes
                    .Where(m => m.Contexto == ContextoFinanceiro.Loja && m.Valor < 0)
                    .Sum(m => m.Valor),
                TotalLoja = movimentacoes
                    .Where(m => m.Contexto == ContextoFinanceiro.Loja)
                    .Sum(m => m.Valor),
                TotalDebito = movimentacoes
                    .Where(m => m.MeioPagamento == MeioPagamento.CartaoDebito)
                    .Sum(m => m.Valor),
                TotalCredito = movimentacoes
                    .Where(m => m.MeioPagamento == MeioPagamento.CartaoCredito)
                    .Sum(m => m.Valor)
            };
        }



        public async Task<ResumoAnualDto> ObterResumoAnual(int ano)
        {
            var movimentacoes = await _context.MovimentacoesFinanceiro
                .Where(m => m.Data.Year == ano)
                .ToListAsync();

            return new ResumoAnualDto
            {
                TotalEntradas = movimentacoes
                    .Where(m => m.Valor > 0)
                    .Sum(m => m.Valor),
                TotalSaidas = movimentacoes
                    .Where(m => m.Valor < 0)
                    .Sum(m => m.Valor),
                TotalPessoal = movimentacoes
                    .Where(m => m.Contexto == ContextoFinanceiro.Pessoal)
                    .Sum(m => m.Valor),
                TotalLoja = movimentacoes
                    .Where(m => m.Contexto == ContextoFinanceiro.Loja)
                    .Sum(m => m.Valor),
                TotalDebito = movimentacoes
                    .Where(m => m.MeioPagamento == MeioPagamento.CartaoDebito)
                    .Sum(m => m.Valor),
                TotalCredito = movimentacoes
                    .Where(m => m.MeioPagamento == MeioPagamento.CartaoCredito)
                    .Sum(m => m.Valor)
            };
        }

        public async Task<List<MovimentacaoFinanceiro>> ObterMovimentacoesMensais(
            int ano,
            int mes,
            int? tipo,
            ContextoFinanceiro? contexto,
            MeioPagamento? meioPagamento)
        {
            var query = _context.MovimentacoesFinanceiro
                .Where(m => m.Data.Year == ano && m.Data.Month == mes);

            if (contexto.HasValue)
                query = query.Where(m => m.Contexto == contexto.Value);

            if (meioPagamento.HasValue)
                query = query.Where(m => m.MeioPagamento == meioPagamento.Value);

            if (tipo == 1) // Entradas
                query = query.Where(m => m.Valor > 0);
            else if (tipo == 2) // Saídas
                query = query.Where(m => m.Valor < 0);

            return await query
                .OrderByDescending(m => m.Data)
                .ToListAsync();
        }

        public async Task<bool> AtualizarMovimentacao(int id, MovimentacaoFinanceiro dto)
        {
            var mov = await _context.MovimentacoesFinanceiro.FindAsync(id);

            if (mov == null)
                return false;

            mov.Descricao = dto.Descricao;
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

            if (mov == null)
                return false;

            _context.MovimentacoesFinanceiro.Remove(mov);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ImportResultDto> ImportarDadosCsv(string caminhoArquivo, int? ano = null, int? mes = null)
        {
            var result = new ImportResultDto();

            // Ler sempre em UTF-8 — arquivo sem problemas de codificação
            var text = await File.ReadAllTextAsync(caminhoArquivo, Encoding.UTF8);
            var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            var ptBr = CultureInfo.GetCultureInfo("pt-BR");
            int imported = 0;

            int targetMonth = mes ?? DetectMonthFromFileName(caminhoArquivo) ?? DateTime.Now.Month;
            int targetYear = ano ?? DateTime.Now.Year;

            for (int i = 0; i < lines.Length; i++)
            {
                var lineNumber = i + 1;
                var line = lines[i];

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var trimmed = line.Trim();
                var upper = trimmed.ToUpperInvariant();

                // Ignorar cabeçalhos e linhas de totais
                if (upper.Contains("DESCRI") || upper.Contains("TOTAL") || upper.Contains("SALDO"))
                    continue;

                var parts = trimmed.Split(';');

                // Função local para processar cada "lado" (0 = esquerda, 5 = direita)
                void ProcessSide(int startIndex, string side)
                {
                    try
                    {
                        if (parts.Length <= startIndex)
                            return;

                        var descricao = parts[startIndex].Trim();
                        if (string.IsNullOrWhiteSpace(descricao))
                            return;

                        var descricaoUpper = descricao.ToUpperInvariant();
                        if (descricaoUpper.Contains("TOTAL") || descricaoUpper.Contains("SALDO"))
                            return;

                        // Valor
                        var valorStr = parts.Length > startIndex + 1 ? parts[startIndex + 1].Trim() : string.Empty;
                        if (string.IsNullOrWhiteSpace(valorStr))
                        {
                            result.Errors.Add(new ParsingError { LineNumber = lineNumber, Side = side, RawText = line, Message = "Valor vazio" });
                            return;
                        }

                        if (!decimal.TryParse(valorStr, NumberStyles.Number | NumberStyles.AllowLeadingSign, ptBr, out var valor))
                        {
                            // Tenta limpar e reparsear (substitui ponto de milhar)
                            var clean = valorStr.Replace(".", string.Empty).Replace(",", ".");
                            if (!decimal.TryParse(clean, NumberStyles.Number | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out valor))
                            {
                                result.Errors.Add(new ParsingError { LineNumber = lineNumber, Side = side, RawText = line, Message = $"Valor inválido: '{valorStr}'" });
                                return;
                            }
                        }

                        // Data (pode ser apenas o dia)
                        DateTime data;
                        var dataStr = parts.Length > startIndex + 2 ? parts[startIndex + 2].Trim() : string.Empty;
                        if (int.TryParse(dataStr, out var day))
                        {
                            try { data = new DateTime(targetYear, targetMonth, day); }
                            catch { data = DateTime.Now; }
                        }
                        else if (DateTime.TryParse(dataStr, ptBr, DateTimeStyles.None, out var parsed))
                        {
                            data = parsed;
                        }
                        else
                        {
                            data = DateTime.Now;
                        }

                        // Tipo -> Contexto
                        var tipoStr = parts.Length > startIndex + 3 ? parts[startIndex + 3].Trim() : string.Empty;
                        ContextoFinanceiro contexto = ContextoFinanceiro.Pessoal;
                        if (!string.IsNullOrWhiteSpace(tipoStr))
                        {
                            if (tipoStr.ToLowerInvariant().Contains("loja"))
                                contexto = ContextoFinanceiro.Loja;
                            else if (tipoStr.ToLowerInvariant().Contains("pessoal"))
                                contexto = ContextoFinanceiro.Pessoal;
                        }

                        // Inferir MeioPagamento a partir da descrição
                        MeioPagamento meio = MeioPagamento.Pix;
                        var descLower = descricao.ToLowerInvariant();
                        if (descLower.Contains("cartao") || descLower.Contains("cartão") || descLower.Contains("credito") || descLower.Contains("crédito"))
                            meio = MeioPagamento.CartaoCredito;
                        else if (descLower.Contains("debito") || descLower.Contains("débito"))
                            meio = MeioPagamento.CartaoDebito;
                        else if (descLower.Contains("pix"))
                            meio = MeioPagamento.Pix;

                        // Limpar e tentar extrair apenas o nome quando aplicável
                        descricao = CleanAndExtractName(descricao);

                        // Truncar descrição a um tamanho razoável
                        const int maxDesc = 250;
                        if (descricao.Length > maxDesc)
                            descricao = descricao.Substring(0, maxDesc);

                        var mov = new MovimentacaoFinanceiro
                        {
                            Descricao = descricao,
                            Valor = valor,
                            Contexto = contexto,
                            MeioPagamento = meio,
                            Data = data
                        };

                        _context.MovimentacoesFinanceiro.Add(mov);
                        imported++;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(new ParsingError { LineNumber = lineNumber, Side = side, RawText = line, Message = ex.Message });
                    }
                }

                // Processa lado esquerdo
                ProcessSide(0, "left");
                // Processa lado direito (no CSV há uma coluna extra vazia, então o segundo bloco começa em 5)
                ProcessSide(5, "right");
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Se falhar no Save, devolve erro geral
                result.Errors.Add(new ParsingError { LineNumber = -1, Side = "save", RawText = string.Empty, Message = ex.Message });
            }

            result.Imported = imported;
            return result;
        }

        private int? DetectMonthFromFileName(string path)
        {
            var name = Path.GetFileName(path)?.ToUpperInvariant() ?? string.Empty;
            var months = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"JANEIRO", 1}, {"FEVEREIRO", 2}, {"MARCO", 3}, {"MARÇO", 3}, {"ABRIL", 4}, {"MAIO", 5}, {"JUNHO", 6},
                {"JULHO", 7}, {"AGOSTO", 8}, {"SETEMBRO", 9}, {"OUTUBRO", 10}, {"NOVEMBRO", 11}, {"DEZEMBRO", 12}
            };

            foreach (var kv in months)
            {
                if (name.Contains(kv.Key))
                    return kv.Value;
            }

            return null;
        }

        private static string CleanAndExtractName(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                return descricao ?? string.Empty;

            var d = descricao.Trim();

            // Remove bullets e sequências comuns de encoding
            d = d.Replace("â¢", " ").Replace("Â", " ").Replace("�", " ").Replace("\uFFFD", " ");
            d = Regex.Replace(d, @"\s+", " ").Trim();

            // Padrões para extrair nome
            var patterns = new[]
            {
                new Regex(@"(?:Transfer(?:ê|e)ncia|Transfer|Transferência).* -\s*(?<name>[^-;]+?)\s* -", RegexOptions.IgnoreCase),
                new Regex(@"(?:Transfer(?:ê|e)ncia|Transfer|Transferência).* -\s*(?<name>[^;]+)$", RegexOptions.IgnoreCase),
                new Regex(@"Reembolso.* -\s*(?<name>[^-;]+?)\s* -", RegexOptions.IgnoreCase),
                new Regex(@"Compra.* -\s*(?<name>[^;]+)$", RegexOptions.IgnoreCase),
                new Regex(@"-\s*(?<name>[A-Z][^;\-]{2,})\s* -", RegexOptions.IgnoreCase)
            };

            foreach (var rx in patterns)
            {
                var m = rx.Match(d);
                if (m.Success && m.Groups["name"] != null)
                {
                    var candidate = m.Groups["name"].Value.Trim();

                    // Remove parcelas / palavras indesejadas
                    candidate = Regex.Replace(candidate, @"parcel\w*.*$", "", RegexOptions.IgnoreCase).Trim();
                    candidate = Regex.Replace(candidate, @"Ag[eê]ncia.*$", "", RegexOptions.IgnoreCase).Trim();
                    candidate = Regex.Replace(candidate, @"Conta.*$", "", RegexOptions.IgnoreCase).Trim();

                    // Rejeita se contiver muita informação não-nome (ex.: números de conta)
                    if (Regex.IsMatch(candidate, @"\d") || candidate.Length < 2)
                        continue;

                    return candidate;
                }
            }

            // Fallback: se contém ' - ' pega a segunda parte quando ela for um nome (sem palavras como Agencia/Conta)
            var parts = d.Split('-');
            if (parts.Length >= 2)
            {
                var maybe = parts[1].Trim();
                if (!Regex.IsMatch(maybe, "Conta|Ag[eê]ncia|NU PAGAMENTOS|PAGSEGURO|EBANX|MERCADE" , RegexOptions.IgnoreCase)
                    && !Regex.IsMatch(maybe, "\\d"))
                {
                    if (maybe.Length > 0 && maybe.Length <= 200)
                        return maybe;
                }
            }

            return d;
        }

    }
}