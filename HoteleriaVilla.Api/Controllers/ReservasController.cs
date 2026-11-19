using HoteleriaVilla.Api.DTOs;
using HoteleriaVilla.Domain.Entities;
using HoteleriaVilla.Domain.Enums;
using HoteleriaVilla.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoteleriaVilla.Api.Controllers;

[ApiController]
[Route("api/reservas")]
public class ReservasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReservasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 3. POST /api/reservas
    [HttpPost]
    public async Task<IActionResult> CrearReserva([FromBody] CrearReservaRequest request)
    {
        if (request.HabitacionesIds == null || !request.HabitacionesIds.Any())
            return BadRequest(new { mensaje = "Debe seleccionar al menos una habitación." });

        var titular = await _context.Huespedes.FindAsync(request.HuespedTitularId);
        if (titular == null)
            return NotFound(new { mensaje = "El huésped titular no existe." });

        var ocupadas = await _context.ReservaHabitaciones
            .Where(rh => request.HabitacionesIds.Contains(rh.HabitacionId)
                      && rh.Reserva.Estado != EstadoReserva.Cancelada
                      && rh.Reserva.Estado != EstadoReserva.CheckOut
                      && request.FechaEntrada.Date < rh.Reserva.FechaSalida
                      && request.FechaSalida.Date > rh.Reserva.FechaEntrada)
            .AnyAsync();

        if (ocupadas)
            return Conflict(new { mensaje = "Una o más habitaciones no están disponibles en las fechas indicadas." });

        var habitaciones = await _context.Habitaciones
            .Include(h => h.TipoHabitacion)
            .Where(h => request.HabitacionesIds.Contains(h.Id))
            .ToListAsync();

        if (habitaciones.Count != request.HabitacionesIds.Count)
            return BadRequest(new { mensaje = "Alguna de las habitaciones indicadas no existe." });

        var codigo = $"HV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
        var reserva = new Reserva(codigo, request.HuespedTitularId, request.FechaEntrada, request.FechaSalida);

        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync();

        foreach (var hab in habitaciones)
        {
            reserva.AgregarHabitacion(hab.Id, hab.TipoHabitacion.TarifaUsd);
        }

        reserva.AgregarHuesped(request.HuespedTitularId, true);
        if (request.AcompanantesIds != null)
        {
            foreach (var acompId in request.AcompanantesIds.Where(id => id != request.HuespedTitularId).Distinct())
            {
                reserva.AgregarHuesped(acompId, false);
            }
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPorCodigo), new { codigoReserva = reserva.CodigoReserva }, new
        {
            reserva.Id,
            reserva.CodigoReserva,
            Estado = reserva.Estado.ToString()
        });
    }

    // 4. POST /api/reservas/{id}/checkin
    [HttpPost("{id}/checkin")]
    public async Task<IActionResult> RealizarCheckIn(int id)
    {
        var reserva = await _context.Reservas
            .Include(r => r.Habitaciones)
            .ThenInclude(rh => rh.Habitacion)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reserva == null)
            return NotFound(new { mensaje = "Reserva no encontrada." });

        try
        {
            reserva.RealizarCheckIn();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }

        foreach (var rh in reserva.Habitaciones)
        {
            var bitacora = rh.Habitacion.CambiarEstado(EstadoHabitacion.Ocupada, $"Check-in Reserva {reserva.CodigoReserva}");
            _context.BitacoraHabitaciones.Add(bitacora);
        }

        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "Check-in realizado exitosamente.", codigoReserva = reserva.CodigoReserva });
    }

    // 5. POST /api/reservas/{id}/consumos
    [HttpPost("{id}/consumos")]
    public async Task<IActionResult> RegistrarConsumo(int id, [FromBody] RegistrarConsumoRequest request)
    {
        var reserva = await _context.Reservas.FindAsync(id);
        if (reserva == null)
            return NotFound(new { mensaje = "Reserva no encontrada." });

        if (reserva.Estado != EstadoReserva.CheckIn)
            return BadRequest(new { mensaje = "Solo se pueden cargar consumos a reservas con Check-In activo." });

        var servicio = await _context.ServiciosAdicionales.FindAsync(request.ServicioId);
        if (servicio == null)
            return NotFound(new { mensaje = "Servicio adicional no encontrado." });

        var consumo = new ConsumoReserva(id, servicio.Id, request.Cantidad, servicio.PrecioUnitarioUsd);
        _context.ConsumosReserva.Add(consumo);

        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "Consumo registrado.", totalUsd = servicio.PrecioUnitarioUsd * request.Cantidad });
    }

    // 6. POST /api/reservas/{id}/checkout
    [HttpPost("{id}/checkout")]
    public async Task<IActionResult> RealizarCheckOut(int id)
    {
        var reserva = await _context.Reservas
            .Include(r => r.Habitaciones).ThenInclude(rh => rh.Habitacion)
            .Include(r => r.Consumos)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reserva == null)
            return NotFound(new { mensaje = "Reserva no encontrada." });

        var tasaHoy = await _context.TasasCambio
            .Where(t => t.FechaVigencia.Date <= DateTime.UtcNow.Date && t.Fuente == "BCV")
            .OrderByDescending(t => t.FechaVigencia)
            .FirstOrDefaultAsync();

        if (tasaHoy == null)
            return BadRequest(new { mensaje = "No hay tasa oficial BCV vigente para generar la factura." });

        try
        {
            reserva.RealizarCheckOut();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }

        int noches = reserva.CalcularNoches();
        decimal totalHospedajeUsd = reserva.Habitaciones.Sum(h => h.TarifaAcordadaUsd * noches);
        decimal totalConsumosUsd = reserva.Consumos.Sum(c => c.PrecioAplicadoUsd * c.Cantidad);
        decimal subTotalUsd = totalHospedajeUsd + totalConsumosUsd;

        var correlativo = await _context.Facturas.CountAsync() + 1;
        var factura = new Factura(
            reservaId: reserva.Id,
            tasaCambioId: tasaHoy.Id,
            numeroFactura: $"FAC-{correlativo:D6}",
            numeroControl: $"00-{correlativo:D8}",
            tasaBcvAplicada: tasaHoy.ValorTasa,
            subTotalUsd: subTotalUsd
        );

        _context.Facturas.Add(factura);

        foreach (var rh in reserva.Habitaciones)
        {
            var bitacora = rh.Habitacion.CambiarEstado(EstadoHabitacion.Limpieza, $"Check-out Reserva {reserva.CodigoReserva}");
            _context.BitacoraHabitaciones.Add(bitacora);
        }

        await _context.SaveChangesAsync();
        return Ok(new
        {
            mensaje = "Check-out completado y factura generada.",
            facturaId = factura.Id,
            factura.NumeroFactura,
            factura.SubTotalUsd,
            factura.TotalBs
        });
    }

    // 10. GET /api/reservas/codigo/{codigoReserva}
    [HttpGet("codigo/{codigoReserva}")]
    public async Task<IActionResult> GetPorCodigo(string codigoReserva)
    {
        var reserva = await _context.Reservas
            .Include(r => r.HuespedTitular)
            .Include(r => r.Huespedes).ThenInclude(hp => hp.Huesped)
            .Include(r => r.Habitaciones).ThenInclude(rh => rh.Habitacion).ThenInclude(h => h.TipoHabitacion)
            .Include(r => r.Consumos).ThenInclude(c => c.Servicio)
            .FirstOrDefaultAsync(r => r.CodigoReserva == codigoReserva);

        if (reserva == null)
            return NotFound(new { mensaje = "Reserva no encontrada." });

        return Ok(new
        {
            reserva.Id,
            reserva.CodigoReserva,
            Estado = reserva.Estado.ToString(),
            reserva.FechaEntrada,
            reserva.FechaSalida,
            Titular = $"{reserva.HuespedTitular.Nombres} {reserva.HuespedTitular.Apellidos}",
            Huespedes = reserva.Huespedes.Select(hp => new
            {
                hp.Huesped.Cedula,
                NombreCompleto = $"{hp.Huesped.Nombres} {hp.Huesped.Apellidos}",
                hp.EsTitular
            }),
            Habitaciones = reserva.Habitaciones.Select(rh => new
            {
                rh.Habitacion.Numero,
                Tipo = rh.Habitacion.TipoHabitacion.Nombre,
                rh.TarifaAcordadaUsd
            }),
            Consumos = reserva.Consumos.Select(c => new
            {
                Servicio = c.Servicio.Nombre,
                c.Cantidad,
                c.PrecioAplicadoUsd,
                TotalLineaUsd = c.Cantidad * c.PrecioAplicadoUsd
            })
        });
    }

    // 11. DELETE /api/reservas/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelarReserva(int id)
    {
        var reserva = await _context.Reservas.FindAsync(id);
        if (reserva == null)
            return NotFound(new { mensaje = "Reserva no encontrada." });

        try
        {
            reserva.Cancelar();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }

        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "Reserva cancelada lógicamente." });
    }
}
