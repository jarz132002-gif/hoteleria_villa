namespace HoteleriaVilla.Domain.Entities;

public class TasaCambio
{
    public int Id { get; private set; }
    public DateTime FechaVigencia { get; private set; }
    public string MonedaOrigen { get; private set; } = "USD";
    public string MonedaDestino { get; private set; } = "VES";
    public decimal ValorTasa { get; private set; }
    public string Fuente { get; private set; } = "BCV";

    private TasaCambio() { }

    public TasaCambio(DateTime fechaVigencia, decimal valorTasa, string fuente = "BCV", string monedaOrigen = "USD", string monedaDestino = "VES")
    {
        if (valorTasa <= 0) throw new ArgumentException("El valor de la tasa debe ser mayor a cero.");
        FechaVigencia = fechaVigencia.Date;
        ValorTasa = valorTasa;
        Fuente = fuente;
        MonedaOrigen = monedaOrigen;
        MonedaDestino = monedaDestino;
    }
}
