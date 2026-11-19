using HoteleriaVilla.Domain.Enums;

namespace HoteleriaVilla.Domain.Entities;

public class Reserva
{
    public int Id { get; private set; }
    public string CodigoReserva { get; private set; } = null!;
    public int HuespedTitularId { get; private set; }
    public DateTime FechaEntrada { get; private set; }
    public DateTime FechaSalida { get; private set; }
    public EstadoReserva Estado { get; private set; }

    public Huesped HuespedTitular { get; private set; } = null!;
    private readonly List<ReservaHabitacion> _habitaciones = new();
    public IReadOnlyCollection<ReservaHabitacion> Habitaciones => _habitaciones.AsReadOnly();

    private readonly List<HuespedPorReserva> _huespedes = new();
    public IReadOnlyCollection<HuespedPorReserva> Huespedes => _huespedes.AsReadOnly();

    private readonly List<ConsumoReserva> _consumos = new();
    public IReadOnlyCollection<ConsumoReserva> Consumos => _consumos.AsReadOnly();

    private Reserva() { }

    public Reserva(string codigoReserva, int huespedTitularId, DateTime fechaEntrada, DateTime fechaSalida)
    {
        if (fechaSalida.Date <= fechaEntrada.Date)
            throw new ArgumentException("La fecha de salida debe ser posterior a la fecha de entrada.");

        CodigoReserva = codigoReserva;
        HuespedTitularId = huespedTitularId;
        FechaEntrada = fechaEntrada.Date;
        FechaSalida = fechaSalida.Date;
        Estado = EstadoReserva.Confirmada;
    }

    public void AgregarHabitacion(int habitacionId, decimal tarifaAcordadaUsd)
    {
        _habitaciones.Add(new ReservaHabitacion(Id, habitacionId, tarifaAcordadaUsd));
    }

    public void AgregarHuesped(int huespedId, bool esTitular)
    {
        _huespedes.Add(new HuespedPorReserva(Id, huespedId, esTitular));
    }

    public void RealizarCheckIn()
    {
        if (Estado != EstadoReserva.Confirmada)
            throw new InvalidOperationException("Solo se puede hacer check-in a reservas en estado Confirmada.");

        Estado = EstadoReserva.CheckIn;
    }

    public void RealizarCheckOut()
    {
        if (Estado != EstadoReserva.CheckIn)
            throw new InvalidOperationException("Solo se puede hacer check-out a reservas con CheckIn activo.");

        Estado = EstadoReserva.CheckOut;
    }

    public void Cancelar()
    {
        if (Estado != EstadoReserva.Confirmada)
            throw new InvalidOperationException("Solo se pueden cancelar reservas en estado Confirmada antes del check-in.");

        Estado = EstadoReserva.Cancelada;
    }

    public int CalcularNoches()
    {
        var noches = (FechaSalida.Date - FechaEntrada.Date).Days;
        return noches <= 0 ? 1 : noches;
    }
}
