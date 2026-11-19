using HoteleriaVilla.Domain.Enums;

namespace HoteleriaVilla.Domain.Entities;

public class Habitacion
{
    public int Id { get; private set; }
    public string Numero { get; private set; } = null!;
    public int Piso { get; private set; }
    public int TipoHabitacionId { get; private set; }
    public EstadoHabitacion Estado { get; private set; }

    public TipoHabitacion TipoHabitacion { get; private set; } = null!;

    private Habitacion() { }

    public Habitacion(string numero, int piso, int tipoHabitacionId)
    {
        if (string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("El número de habitación es obligatorio.");
        Numero = numero;
        Piso = piso;
        TipoHabitacionId = tipoHabitacionId;
        Estado = EstadoHabitacion.Disponible;
    }

    public BitacoraHabitacion CambiarEstado(EstadoHabitacion nuevoEstado, string observacion)
    {
        var estadoAnterior = Estado.ToString();
        Estado = nuevoEstado;
        return new BitacoraHabitacion(Id, estadoAnterior, nuevoEstado.ToString(), observacion);
    }
}
