namespace HoteleriaVilla.Domain.Entities;

public class ServicioAdicional
{
    public int Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public decimal PrecioUnitarioUsd { get; private set; }
    public string Categoria { get; private set; } = null!;

    private ServicioAdicional() { }

    public ServicioAdicional(string nombre, decimal precioUnitarioUsd, string categoria)
    {
        Nombre = nombre;
        PrecioUnitarioUsd = precioUnitarioUsd;
        Categoria = categoria;
    }
}
