namespace HoteleriaVilla.Domain.Entities;

public class BitacoraHabitacion
{
    public int Id { get; private set; }
    public int HabitacionId { get; private set; }
    public string EstadoAnterior { get; private set; } = null!;
    public string EstadoNuevo { get; private set; } = null!;
    public DateTime FechaRegistro { get; private set; }
    public string Observacion { get; private set; } = null!;

    public Habitacion Habitacion { get; private set; } = null!;

    private BitacoraHabitacion() { }

    public BitacoraHabitacion(int habitacionId, string estadoAnterior, string estadoNuevo, string observacion)
    {
        HabitacionId = habitacionId;
        EstadoAnterior = estadoAnterior;
        EstadoNuevo = estadoNuevo;
        FechaRegistro = DateTime.UtcNow;
        Observacion = observacion;
    }
}
