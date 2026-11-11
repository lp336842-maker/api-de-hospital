using ClinicaPacientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicaPacientes.Infrastructure.Data;

public class ClinicaDbContext(DbContextOptions<ClinicaDbContext> options) : DbContext(options)
{
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Especialidad> Especialidades => Set<Especialidad>();
    public DbSet<Medico> Medicos => Set<Medico>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<HistorialMedico> HistorialesMedicos => Set<HistorialMedico>();
    public DbSet<Medicamento> Medicamentos => Set<Medicamento>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<DetalleFactura> DetallesFactura => Set<DetalleFactura>();
    public DbSet<SeguroMedico> SegurosMedicos => Set<SeguroMedico>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Paciente>().HasIndex(x => x.Documento).IsUnique();
        builder.Entity<Usuario>().HasIndex(x => x.Correo).IsUnique();
        builder.Entity<SeguroMedico>().HasIndex(x => x.NumeroPoliza).IsUnique();
        builder.Entity<Medico>().HasOne(x => x.Especialidad).WithMany(x => x.Medicos).HasForeignKey(x => x.EspecialidadId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Cita>().HasOne(x => x.Paciente).WithMany(x => x.Citas).HasForeignKey(x => x.PacienteId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Cita>().HasOne(x => x.Medico).WithMany(x => x.Citas).HasForeignKey(x => x.MedicoId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<HistorialMedico>().HasOne(x => x.Paciente).WithMany(x => x.Historiales).HasForeignKey(x => x.PacienteId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Factura>().HasOne(x => x.Cita).WithMany(x => x.Facturas).HasForeignKey(x => x.CitaId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<DetalleFactura>().HasOne(x => x.Factura).WithMany(x => x.Detalles).HasForeignKey(x => x.FacturaId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<DetalleFactura>().HasOne(x => x.Medicamento).WithMany(x => x.DetallesFactura).HasForeignKey(x => x.MedicamentoId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<SeguroMedico>().HasOne(x => x.Paciente).WithMany().HasForeignKey(x => x.PacienteId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Medicamento>().Property(x => x.Precio).HasPrecision(12, 2);
        builder.Entity<Factura>().Property(x => x.Total).HasPrecision(12, 2);
        builder.Entity<DetalleFactura>().Property(x => x.PrecioUnitario).HasPrecision(12, 2);
    }
}
