using System.Security.Claims;

public class PlanoAtivoMiddleware
{
    private readonly RequestDelegate _next;

    // rotas que ficam livres mesmo com plano expirado
    private static readonly string[] RotasLiberadas = new[]
    {
        "/api/assinatura/iniciar",
        "/api/webhook/abacatepay"
    };

    public PlanoAtivoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AtelieService atelieService)
    {
        // deixa passar rotas liberadas sem verificar plano
        var path = context.Request.Path.Value?.ToLower();
        if (RotasLiberadas.Any(r => path?.StartsWith(r) == true))
        {
            await _next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdStr = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdStr, out var userId))
            {
                var ativo = await atelieService.AssinaturaAtiva(userId);

                if (!ativo && context.Request.Method != HttpMethods.Get)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        erro = "Plano expirado. Apenas visualização permitida."
                    });

                    return;
                }
            }
        }

        await _next(context);
    }
}