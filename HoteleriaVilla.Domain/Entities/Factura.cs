using HoteleriaVilla.Domain.Enums;

namespace HoteleriaVilla.Domain.Entities;

public class Factura
{
    public int Id { get; private set; }
    public int ReservaId { get; private set; }
    public int TasaCambioId { get; private set; }
    public string NumeroFactura { get; private set; } = null!;
    public string NumeroControl { get; private set; } = null!;
    public DateTime FechaEmision { get; private set; }
    public decimal TasaBcvAplicada { get; private set; }
    public decimal SubTotalUsd { get; private set; }
    public decimal PorcentajeIva { get; private set; }
    public decimal MontoIvaBs { get; private set; }
    public decimal TotalBs { get; private set; }
    public EstadoFactura Estado { get; private set; }

    public Reserva Reserva { get; private set; } = null!;
    public TasaCambio TasaCambio { get; private set; } = null!;
    private readonly List<Pago> _pagos = new();
    public IReadOnlyCollection<Pago> Pagos => _pagos.AsReadOnly();

    private Factura() { }

    public Factura(int reservaId, int tasaCambioId, string numeroFactura, string numeroControl, decimal tasaBcvAplicada, decimal subTotalUsd, decimal porcentajeIva = 16m)
    {
        ReservaId = reservaId;
        TasaCambioId = tasaCambioId;
        NumeroFactura = numeroFactura;
        NumeroControl = numeroControl;
        FechaEmision = DateTime.UtcNow;
        TasaBcvAplicada = tasaBcvAplicada;
        SubTotalUsd = Math.Round(subTotalUsd, 2);
        PorcentajeIva = porcentajeIva;

        var subTotalBs = Math.Round(SubTotalUsd * TasaBcvAplicada, 2);
        MontoIvaBs = Math.Round(subTotalBs * (PorcentajeIva / 100m), 2);
        TotalBs = Math.Round(subTotalBs + MontoIvaBs, 2);
        Estado = EstadoFactura.Emitida;
    }

    public decimal ObtenerTotalUsdConIva()
    {
        return Math.Round(SubTotalUsd * (1 + (PorcentajeIva / 100m)), 2);
    }

    public void MarcarComoPagada()
    {
        Estado = EstadoFactura.Pagada;
    }
}
