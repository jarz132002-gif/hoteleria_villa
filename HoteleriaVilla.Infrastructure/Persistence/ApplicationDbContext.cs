using HoteleriaVilla.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HoteleriaVilla.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TasaCambio> TasasCambio => Set<TasaCambio>();
    public DbSet<TipoHabitacion> TiposHabitacion => Set<TipoHabitacion>();
    public DbSet<Habitacion> Habitaciones => Set<Habitacion>();
    public DbSet<Huesped> Huespedes => Set<Huesped>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<ReservaHabitacion> ReservaHabitaciones => Set<ReservaHabitacion>();
    public DbSet<HuespedPorReserva> HuespedesPorReserva => Set<HuespedPorReserva>();
    public DbSet<ServicioAdicional> ServiciosAdicionales => Set<ServicioAdicional>();
    public DbSet<ConsumoReserva> ConsumosReserva => Set<ConsumoReserva>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<BitacoraHabitacion> BitacoraHabitaciones => Set<BitacoraHabitacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
