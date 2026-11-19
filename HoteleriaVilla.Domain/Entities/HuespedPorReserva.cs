namespace HoteleriaVilla.Domain.Entities;

public class HuespedPorReserva
{
    public int ReservaId { get; private set; }
    public int HuespedId { get; private set; }
    public bool EsTitular { get; private set; }

    public Reserva Reserva { get; private set; } = null!;
    public Huesped Huesped { get; private set; } = null!;

    private HuespedPorReserva() { }

    public HuespedPorReserva(int reservaId, int huespedId, bool esTitular)
    {
        ReservaId = reservaId;
        HuespedId = huespedId;
        EsTitular = esTitular;
    }
}
