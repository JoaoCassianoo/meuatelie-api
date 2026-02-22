using Microsoft.EntityFrameworkCore;
using Atelie.Api.Data;
using Atelie.Api.Entities;

namespace Atelie.Api.Services
{
    public class AssinaturaService
    {
        private readonly AtelieDbContext _context;
        private readonly AbacatePayService _abacateService;

        public AssinaturaService(AtelieDbContext context, AbacatePayService abacateService)
        {
            _context = context;
            _abacateService = abacateService;
        }

        public async Task<(bool Sucesso, string? Erro, string? Url)> IniciarAssinatura(
            Guid userId,
            string email,
            string periodicidade)
        {
            var atelie = await _context.AtelieInfo
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (atelie == null)
                return (false, "Ateliê não encontrado.", null);

            if (atelie.Status == "ativo" && atelie.DataVencimento > DateTime.UtcNow)
                return (false, "Você já tem uma assinatura ativa.", null);

            var (billingId, billingUrl) = await _abacateService.CriarCobrancaAsync(
                userId: userId.ToString(),
                nomeDono: atelie.NomeDono,
                email: email,
                nomeAtelie: atelie.NomeAtelie,
                periocidade: periodicidade
            );

            atelie.BillingId = billingId;
            atelie.BillingUrl = billingUrl;

            await _context.SaveChangesAsync();

            return (true, null, billingUrl);
        }

        public async Task<bool> AtivarAcesso(string userId, string billingId, string externalIdProduto)
        {
            var atelie = await _context.AtelieInfo
                .FirstOrDefaultAsync(a => a.UserId == Guid.Parse(userId));

            if (atelie == null)
                return false;

            var diasDeAcesso = externalIdProduto switch
            {
                "pro-trimestral" => 90,
                "pro-anual"      => 365,
                _                => 30
            };

            atelie.Status = "ativo";
            atelie.Plano = "Pro";
            atelie.DataVencimento = DateTime.UtcNow.AddDays(diasDeAcesso);
            atelie.BillingId = billingId;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}