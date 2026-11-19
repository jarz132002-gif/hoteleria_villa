using HoteleriaVilla.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoteleriaVilla.Api.Controllers;

[ApiController]
[Route("api/tasas-cambio")]
public class TasasCambioController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TasasCambioController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. GET /api/tasas-cambio/hoy
    [HttpGet("hoy")]
    public async Task<IActionResult> GetTasaHoy()
    {
        var hoy = DateTime.UtcNow.Date;
        var tasa = await _context.TasasCambio
            .Where(t => t.FechaVigencia.Date <= hoy && t.Fuente == "BCV")
            .OrderByDescending(t => t.FechaVigencia)
            .FirstOrDefaultAsync();

        if (tasa == null)
            return NotFound(new { mensaje = "No existe tasa oficial del BCV registrada." });

        return Ok(tasa);
    }
}
