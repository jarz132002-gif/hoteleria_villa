namespace HoteleriaVilla.Domain.Entities;

public class ReservaHabitacion
{
    public int ReservaId { get; private set; }
    public int HabitacionId { get; private set; }
    public decimal TarifaAcordadaUsd { get; private set; }

    public Reserva Reserva { get; private set; } = null!;
    public Habitacion Habitacion { get; private set; } = null!;

    private ReservaHabitacion() { }

    public ReservaHabitacion(int reservaId, int habitacionId, decimal tarifaAcordadaUsd)
    {
        ReservaId = reservaId;
        HabitacionId = habitacionId;
        TarifaAcordadaUsd = tarifaAcordadaUsd;
    }
}
