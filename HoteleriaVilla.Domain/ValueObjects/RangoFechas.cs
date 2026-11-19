namespace HoteleriaVilla.Domain.ValueObjects;

public record RangoFechas
{
    public DateTime FechaEntrada { get; }
    public DateTime FechaSalida { get; }

    public RangoFechas(DateTime fechaEntrada, DateTime fechaSalida)
    {
        if (fechaSalida.Date <= fechaEntrada.Date)
            throw new ArgumentException("La fecha de salida debe ser posterior a la fecha de entrada.");

        FechaEntrada = fechaEntrada.Date;
        FechaSalida = fechaSalida.Date;
    }

    public int CantidadNoches => (FechaSalida - FechaEntrada).Days;
}
