using HoteleriaVilla.Domain.Enums;

namespace HoteleriaVilla.Domain.Entities;

public class Pago
{
    public int Id { get; private set; }
    public int FacturaId { get; private set; }
    public int TasaCambioId { get; private set; }
    public MetodoPago MetodoPago { get; private set; }
    public string MonedaPago { get; private set; } = null!;
    public decimal MontoOriginal { get; private set; }
    public decimal TasaAplicada { get; private set; }
    public decimal EquivalenteUsd { get; private set; }
    public string Referencia { get; private set; } = null!;
    public DateTime FechaPago { get; private set; }

    public Factura Factura { get; private set; } = null!;
    public TasaCambio TasaCambio { get; private set; } = null!;

    private Pago() { }

    public Pago(int facturaId, int tasaCambioId, MetodoPago metodoPago, string monedaPago, decimal montoOriginal, decimal tasaAplicada, string referencia)
    {
        if (montoOriginal <= 0) throw new ArgumentException("El monto del pago debe ser mayor a cero.");

        FacturaId = facturaId;
        TasaCambioId = tasaCambioId;
        MetodoPago = metodoPago;
        MonedaPago = monedaPago.ToUpper();
        MontoOriginal = montoOriginal;
        TasaAplicada = tasaAplicada;
        Referencia = referencia;
        FechaPago = DateTime.UtcNow;

        EquivalenteUsd = MonedaPago == "USD"
            ? Math.Round(montoOriginal, 2)
            : Math.Round(montoOriginal / tasaAplicada, 2);
    }
}
