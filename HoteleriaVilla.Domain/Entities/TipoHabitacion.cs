namespace HoteleriaVilla.Domain.Entities;

public class TipoHabitacion
{
    public int Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public int CapacidadMaxima { get; private set; }
    public decimal TarifaUsd { get; private set; }
    public string Descripcion { get; private set; } = null!;

    private TipoHabitacion() { }

    public TipoHabitacion(string nombre, int capacidadMaxima, decimal tarifaUsd, string descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre es obligatorio.");
        if (capacidadMaxima <= 0) throw new ArgumentException("La capacidad máxima debe ser mayor a 0.");
        if (tarifaUsd <= 0) throw new ArgumentException("La tarifa debe ser mayor a 0.");

        Nombre = nombre;
        CapacidadMaxima = capacidadMaxima;
        TarifaUsd = tarifaUsd;
        Descripcion = descripcion;
    }
}
