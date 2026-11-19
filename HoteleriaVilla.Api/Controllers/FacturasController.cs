using HoteleriaVilla.Api.DTOs;
using HoteleriaVilla.Domain.Entities;
using HoteleriaVilla.Domain.Enums;
using HoteleriaVilla.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoteleriaVilla.Api.Controllers;

[ApiController]
[Route("api/facturas")]
public class FacturasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FacturasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 7. GET /api/facturas/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFactura(int id)
    {
        var factura = await _context.Facturas
            .Include(f => f.Reserva).ThenInclude(r => r.HuespedTitular)
            .Include(f => f.Pagos)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (factura == null)
            return NotFound(new { mensaje = "Factura no encontrada." });

        var totalPagadoUsd = factura.Pagos.Sum(p => p.EquivalenteUsd);
        var totalFacturaUsd = factura.ObtenerTotalUsdConIva();

        return Ok(new
        {
            factura.Id,
            factura.NumeroFactura,
            factura.NumeroControl,
            factura.FechaEmision,
            Cliente = $"{factura.Reserva.HuespedTitular.Nombres} {factura.Reserva.HuespedTitular.Apellidos}",
            Cedula = factura.Reserva.HuespedTitular.Cedula,
            factura.TasaBcvAplicada,
            factura.SubTotalUsd,
            factura.PorcentajeIva,
            factura.MontoIvaBs,
            factura.TotalBs,
            TotalUsdConIva = totalFacturaUsd,
            TotalPagadoUsd = totalPagadoUsd,
            SaldoPendienteUsd = Math.Max(0, Math.Round(totalFacturaUsd - totalPagadoUsd, 2)),
            Estado = factura.Estado.ToString(),
            Pagos = factura.Pagos.Select(p => new
            {
                p.Id,
                Metodo = p.MetodoPago.ToString(),
                p.MonedaPago,
                p.MontoOriginal,
                p.TasaAplicada,
                p.EquivalenteUsd,
                p.Referencia,
                p.FechaPago
            })
        });
    }

    // 8. POST /api/facturas/{id}/pagos
    [HttpPost("{id}/pagos")]
    public async Task<IActionResult> RegistrarPago(int id, [FromBody] RegistrarPagoRequest request)
    {
        var factura = await _context.Facturas
            .Include(f => f.Pagos)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (factura == null)
            return NotFound(new { mensaje = "Factura no encontrada." });

        if (factura.Estado == EstadoFactura.Pagada)
            return BadRequest(new { mensaje = "Esta factura ya se encuentra pagada en su totalidad." });

        var tasaHoy = await _context.TasasCambio
            .Where(t => t.FechaVigencia.Date <= DateTime.UtcNow.Date && t.Fuente == "BCV")
            .OrderByDescending(t => t.FechaVigencia)
            .FirstOrDefaultAsync();

        if (tasaHoy == null)
            return BadRequest(new { mensaje = "No hay tasa BCV registrada para procesar el pago." });

        decimal tasaAplicar = request.MonedaPago.ToUpper() == "USD" ? 1m : tasaHoy.ValorTasa;
        var pago = new Pago(
            facturaId: factura.Id,
            tasaCambioId: tasaHoy.Id,
            metodoPago: request.MetodoPago,
            monedaPago: request.MonedaPago,
            montoOriginal: request.MontoOriginal,
            tasaAplicada: tasaAplicar,
            referencia: request.Referencia
        );

        var totalFacturaUsd = factura.ObtenerTotalUsdConIva();
        var pagadoPrevioUsd = factura.Pagos.Sum(p => p.EquivalenteUsd);
        var saldoPendiente = Math.Round(totalFacturaUsd - pagadoPrevioUsd, 2);

        if (pago.EquivalenteUsd > saldoPendiente + 0.05m)
            return BadRequest(new { mensaje = $"El monto excede el saldo pendiente ({saldoPendiente} USD)." });

        _context.Pagos.Add(pago);

        if (pagadoPrevioUsd + pago.EquivalenteUsd >= totalFacturaUsd - 0.01m)
        {
            factura.MarcarComoPagada();
        }

        await _context.SaveChangesAsync();
        return Ok(new
        {
            mensaje = "Pago registrado correctamente.",
            equivalenteUsd = pago.EquivalenteUsd,
            estadoFactura = factura.Estado.ToString()
        });
    }
}
