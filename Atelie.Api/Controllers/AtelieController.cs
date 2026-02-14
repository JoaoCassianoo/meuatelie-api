using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AtelieController : ControllerBase
{
    private readonly AtelieService _service;

    public AtelieController(AtelieService service)
    {
        _service = service;
    }

    private Guid UserId =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);


    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var atelie = await _service.Obter(UserId);

        if (atelie == null)
            return NotFound();

        return Ok(atelie);
    }


    [HttpPut]
    public async Task<IActionResult> Update(UpdateAtelieDto dto)
    {
        await _service.Atualizar(UserId, dto);
        return NoContent();
    }


    [HttpGet("assinatura-ativa")]
    public async Task<IActionResult> AssinaturaAtiva()
    {
        var ativa = await _service.AssinaturaAtiva(UserId);
        return Ok(new { ativa });
    }
}
