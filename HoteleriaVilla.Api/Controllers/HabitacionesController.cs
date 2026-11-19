using HoteleriaVilla.Api.DTOs;
using HoteleriaVilla.Domain.Enums;
using HoteleriaVilla.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoteleriaVilla.Api.Controllers;

[ApiController]
[Route("api/habitaciones")]
public class HabitacionesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HabitacionesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 2. GET /api/habitaciones/disponibles
    [HttpGet("disponibles")]
    public async Task<IActionResult> GetDisponibles(
        [FromQuery] DateTime fechaEntrada,
        [FromQuery] DateTime fechaSalida,
        [FromQuery] int? tipoHabitacionId)
    {
        if (fechaSalida.Date <= fechaEntrada.Date)
            return BadRequest(new { mensaje = "La fecha de salida debe ser mayor a la fecha de entrada." });

        var habitacionesOcupadasIds = await _context.ReservaHabitaciones
            .Where(rh => rh.Reserva.Estado != EstadoReserva.Cancelada
                      && rh.Reserva.Estado != EstadoReserva.CheckOut
                      && fechaEntrada.Date < rh.Reserva.FechaSalida
                      && fechaSalida.Date > rh.Reserva.FechaEntrada)
            .Select(rh => rh.HabitacionId)
            .Distinct()
            .ToListAsync();

        var query = _context.Habitaciones
            .Include(h => h.TipoHabitacion)
            .Where(h => !habitacionesOcupadasIds.Contains(h.Id)
                     && h.Estado != EstadoHabitacion.Mantenimiento);

        if (tipoHabitacionId.HasValue)
            query = query.Where(h => h.TipoHabitacionId == tipoHabitacionId.Value);

        var disponibles = await query.Select(h => new
        {
            h.Id,
            h.Numero,
            h.Piso,
            Estado = h.Estado.ToString(),
            TipoHabitacion = h.TipoHabitacion.Nombre,
            h.TipoHabitacion.CapacidadMaxima,
            h.TipoHabitacion.TarifaUsd
        }).ToListAsync();

        return Ok(disponibles);
    }

    // 9. PUT /api/habitaciones/{id}/estado
    [HttpPut("{id}/estado")]
    public async Task<IActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoHabitacionRequest request)
    {
        var habitacion = await _context.Habitaciones.FindAsync(id);
        if (habitacion == null)
            return NotFound(new { mensaje = "Habitación no encontrada." });

        var registroBitacora = habitacion.CambiarEstado(request.NuevoEstado, request.Observacion);
        _context.BitacoraHabitaciones.Add(registroBitacora);

        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "Estado actualizado y registrado en bitácora.", estadoActual = habitacion.Estado.ToString() });
    }
}
