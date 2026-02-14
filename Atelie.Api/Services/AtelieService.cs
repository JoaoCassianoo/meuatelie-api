using Atelie.Api.Data;
using Microsoft.EntityFrameworkCore;

public class AtelieService
{
    private readonly AtelieDbContext _context;

    public AtelieService(AtelieDbContext context)
    {
        _context = context;
    }

    public async Task<AtelieInfo?> Obter(Guid userId)
    {
        return await _context.AtelieInfo
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task Atualizar(Guid userId, UpdateAtelieDto dto)
    {
        var atelie = await _context.AtelieInfo
            .FirstAsync(x => x.UserId == userId);

        atelie.NomeAtelie = dto.NomeAtelie;
        atelie.NomeDono = dto.NomeDono;

        await _context.SaveChangesAsync();
    }

    public async Task<bool> AssinaturaAtiva(Guid userId)
    {
        var atelie = await _context.AtelieInfo
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (atelie == null)
            return false;

        // regra real
        return atelie.DataVencimento >= DateTime.UtcNow.Date;
    }


}