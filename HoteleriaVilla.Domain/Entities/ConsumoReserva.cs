namespace HoteleriaVilla.Domain.Entities;

public class ConsumoReserva
{
    public int Id { get; private set; }
    public int ReservaId { get; private set; }
    public int ServicioId { get; private set; }
    public int Cantidad { get; private set; }
    public decimal PrecioAplicadoUsd { get; private set; }
    public DateTime FechaConsumo { get; private set; }

    public Reserva Reserva { get; private set; } = null!;
    public ServicioAdicional Servicio { get; private set; } = null!;

    private ConsumoReserva() { }

    public ConsumoReserva(int reservaId, int servicioId, int cantidad, decimal precioAplicadoUsd)
    {
        if (cantidad <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");
        ReservaId = reservaId;
        ServicioId = servicioId;
        Cantidad = cantidad;
        PrecioAplicadoUsd = precioAplicadoUsd;
        FechaConsumo = DateTime.UtcNow;
    }
}
