namespace HoteleriaVilla.Domain.Enums;

public enum EstadoHabitacion
{
    Disponible,
    Ocupada,
    Mantenimiento,
    Limpieza
}

public enum EstadoReserva
{
    Confirmada,
    CheckIn,
    CheckOut,
    Cancelada
}

public enum EstadoFactura
{
    Emitida,
    Pagada,
    Anulada
}

public enum MetodoPago
{
    Zelle,
    EfectivoUsd,
    PagoMovil,
    PuntoVenta
}
