using HoteleriaVilla.Domain.Enums;

namespace HoteleriaVilla.Api.DTOs;

public record CrearReservaRequest(
    int HuespedTitularId,
    DateTime FechaEntrada,
    DateTime FechaSalida,
    List<int> HabitacionesIds,
    List<int>? AcompanantesIds
);

public record RegistrarConsumoRequest(
    int ServicioId,
    int Cantidad
);

public record RegistrarPagoRequest(
    MetodoPago MetodoPago,
    string MonedaPago,
    decimal MontoOriginal,
    string Referencia
);

public record ActualizarEstadoHabitacionRequest(
    EstadoHabitacion NuevoEstado,
    string Observacion
);
