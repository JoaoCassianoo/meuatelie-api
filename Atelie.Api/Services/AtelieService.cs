using Atelie.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;

public class AtelieService
{
    private readonly AtelieDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AtelieService(AtelieDbContext context, HttpClient httpClient, IConfiguration configuration)
    {
        _context = context;
        _httpClient = httpClient;
        _configuration = configuration;
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

    public async Task<AtelieInfo> Registrar(RegistroAtelieDto dto)
    {
        var supabaseUrl = _configuration["Supabase:Url"];
        var anonKey = _configuration["Supabase:AnonKey"];

        // Chamar API do Supabase para registrar o usuário
        var authUrl = $"{supabaseUrl}/auth/v1/signup";
        var requestBody = new { email = dto.Email, password = dto.Senha };
        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("apikey", anonKey);

        var response = await _httpClient.PostAsync(authUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao registrar usuário no Supabase: {response.StatusCode} - {errorContent}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        using (JsonDocument doc = JsonDocument.Parse(responseBody))
        {
            var root = doc.RootElement;
            var userIdString = root.GetProperty("user").GetProperty("id").GetString();

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new Exception("Erro ao parsear UUID do usuário retornado pelo Supabase");
            }

            // Criar AtelieInfo com o UUID recebido
            var atelieInfo = new AtelieInfo
            {
                UserId = userId,
                NomeDono = dto.Nome,
                NomeAtelie = dto.AtelieNome,
                Status = "trial",
                Plano = "free",
                DataVencimento = DateTime.UtcNow, // 14 dias de trial
                CreatedAt = DateTime.UtcNow
            };

            _context.AtelieInfo.Add(atelieInfo);
            await _context.SaveChangesAsync();

            return atelieInfo;
        }
    }

}