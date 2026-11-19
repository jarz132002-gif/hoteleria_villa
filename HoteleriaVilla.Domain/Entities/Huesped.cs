namespace HoteleriaVilla.Domain.Entities;

public class Huesped
{
    public int Id { get; private set; }
    public string Cedula { get; private set; } = null!;
    public string Nombres { get; private set; } = null!;
    public string Apellidos { get; private set; } = null!;
    public string Correo { get; private set; } = null!;
    public string Telefono { get; private set; } = null!;
    public string Nacionalidad { get; private set; } = null!;

    private Huesped() { }

    public Huesped(string cedula, string nombres, string apellidos, string correo, string telefono, string nacionalidad)
    {
        Cedula = cedula;
        Nombres = nombres;
        Apellidos = apellidos;
        Correo = correo;
        Telefono = telefono;
        Nacionalidad = nacionalidad;
    }
}
