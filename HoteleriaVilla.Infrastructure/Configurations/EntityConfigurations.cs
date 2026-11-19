using HoteleriaVilla.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoteleriaVilla.Infrastructure.Configurations;

public class TasaCambioConfiguration : IEntityTypeConfiguration<TasaCambio>
{
    public void Configure(EntityTypeBuilder<TasaCambio> builder)
    {
        builder.ToTable("TasasCambio");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.MonedaOrigen).HasMaxLength(10).IsRequired();
        builder.Property(t => t.MonedaDestino).HasMaxLength(10).IsRequired();
        builder.Property(t => t.ValorTasa).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(t => t.Fuente).HasMaxLength(50).IsRequired();
    }
}

public class TipoHabitacionConfiguration : IEntityTypeConfiguration<TipoHabitacion>
{
    public void Configure(EntityTypeBuilder<TipoHabitacion> builder)
    {
        builder.ToTable("TiposHabitacion");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Nombre).HasMaxLength(80).IsRequired();
        builder.Property(t => t.TarifaUsd).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(t => t.Descripcion).HasMaxLength(300);
    }
}

public class HabitacionConfiguration : IEntityTypeConfiguration<Habitacion>
{
    public void Configure(EntityTypeBuilder<Habitacion> builder)
    {
        builder.ToTable("Habitaciones");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Numero).HasMaxLength(10).IsRequired();
        builder.HasIndex(h => h.Numero).IsUnique();
        builder.Property(h => h.Estado).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.HasOne(h => h.TipoHabitacion).WithMany().HasForeignKey(h => h.TipoHabitacionId);
    }
}

public class HuespedConfiguration : IEntityTypeConfiguration<Huesped>
{
    public void Configure(EntityTypeBuilder<Huesped> builder)
    {
        builder.ToTable("Huespedes");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Cedula).HasMaxLength(20).IsRequired();
        builder.HasIndex(h => h.Cedula).IsUnique();
        builder.Property(h => h.Nombres).HasMaxLength(100).IsRequired();
        builder.Property(h => h.Apellidos).HasMaxLength(100).IsRequired();
        builder.Property(h => h.Correo).HasMaxLength(120);
        builder.Property(h => h.Telefono).HasMaxLength(30);
        builder.Property(h => h.Nacionalidad).HasMaxLength(50);
    }
}

public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
{
    public void Configure(EntityTypeBuilder<Reserva> builder)
    {
        builder.ToTable("Reservas");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.CodigoReserva).HasMaxLength(20).IsRequired();
        builder.HasIndex(r => r.CodigoReserva).IsUnique();
        builder.Property(r => r.Estado).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.HasOne(r => r.HuespedTitular).WithMany().HasForeignKey(r => r.HuespedTitularId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ReservaHabitacionConfiguration : IEntityTypeConfiguration<ReservaHabitacion>
{
    public void Configure(EntityTypeBuilder<ReservaHabitacion> builder)
    {
        builder.ToTable("ReservaHabitaciones");
        builder.HasKey(rh => new { rh.ReservaId, rh.HabitacionId });
        builder.Property(rh => rh.TarifaAcordadaUsd).HasColumnType("decimal(18,2)").IsRequired();
        builder.HasOne(rh => rh.Reserva).WithMany(r => r.Habitaciones).HasForeignKey(rh => rh.ReservaId);
        builder.HasOne(rh => rh.Habitacion).WithMany().HasForeignKey(rh => rh.HabitacionId);
    }
}

public class HuespedPorReservaConfiguration : IEntityTypeConfiguration<HuespedPorReserva>
{
    public void Configure(EntityTypeBuilder<HuespedPorReserva> builder)
    {
        builder.ToTable("HuespedesPorReserva");
        builder.HasKey(hp => new { hp.ReservaId, hp.HuespedId });
        builder.HasOne(hp => hp.Reserva).WithMany(r => r.Huespedes).HasForeignKey(hp => hp.ReservaId);
        builder.HasOne(hp => hp.Huesped).WithMany().HasForeignKey(hp => hp.HuespedId);
    }
}

public class ServicioAdicionalConfiguration : IEntityTypeConfiguration<ServicioAdicional>
{
    public void Configure(EntityTypeBuilder<ServicioAdicional> builder)
    {
        builder.ToTable("ServiciosAdicionales");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(s => s.PrecioUnitarioUsd).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(s => s.Categoria).HasMaxLength(50).IsRequired();
    }
}

public class ConsumoReservaConfiguration : IEntityTypeConfiguration<ConsumoReserva>
{
    public void Configure(EntityTypeBuilder<ConsumoReserva> builder)
    {
        builder.ToTable("ConsumosReserva");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.PrecioAplicadoUsd).HasColumnType("decimal(18,2)").IsRequired();
        builder.HasOne(c => c.Reserva).WithMany(r => r.Consumos).HasForeignKey(c => c.ReservaId);
        builder.HasOne(c => c.Servicio).WithMany().HasForeignKey(c => c.ServicioId);
    }
}

public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
{
    public void Configure(EntityTypeBuilder<Factura> builder)
    {
        builder.ToTable("Facturas");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.NumeroFactura).HasMaxLength(30).IsRequired();
        builder.Property(f => f.NumeroControl).HasMaxLength(30).IsRequired();
        builder.Property(f => f.TasaBcvAplicada).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(f => f.SubTotalUsd).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(f => f.PorcentajeIva).HasColumnType("decimal(5,2)").IsRequired();
        builder.Property(f => f.MontoIvaBs).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(f => f.TotalBs).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(f => f.Estado).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.HasOne(f => f.Reserva).WithMany().HasForeignKey(f => f.ReservaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(f => f.TasaCambio).WithMany().HasForeignKey(f => f.TasaCambioId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.ToTable("Pagos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.MetodoPago).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(p => p.MonedaPago).HasMaxLength(10).IsRequired();
        builder.Property(p => p.MontoOriginal).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.TasaAplicada).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(p => p.EquivalenteUsd).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.Referencia).HasMaxLength(80);
        builder.HasOne(p => p.Factura).WithMany(f => f.Pagos).HasForeignKey(p => p.FacturaId);
        builder.HasOne(p => p.TasaCambio).WithMany().HasForeignKey(p => p.TasaCambioId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class BitacoraHabitacionConfiguration : IEntityTypeConfiguration<BitacoraHabitacion>
{
    public void Configure(EntityTypeBuilder<BitacoraHabitacion> builder)
    {
        builder.ToTable("BitacoraHabitaciones");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.EstadoAnterior).HasMaxLength(30).IsRequired();
        builder.Property(b => b.EstadoNuevo).HasMaxLength(30).IsRequired();
        builder.Property(b => b.Observacion).HasMaxLength(300).IsRequired();
        builder.HasOne(b => b.Habitacion).WithMany().HasForeignKey(b => b.HabitacionId);
    }
}
