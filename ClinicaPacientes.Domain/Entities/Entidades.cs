using System.ComponentModel.DataAnnotations;

namespace ClinicaPacientes.Domain.Entities;

public interface IEntity
{
    int Id { get; set; }
}

public class Paciente : IEntity
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Nombres { get; set; } = "";
    [Required, StringLength(100)] public string Apellidos { get; set; } = "";
    [Required, StringLength(30)] public string Documento { get; set; } = "";
    public DateTime FechaNacimiento { get; set; }
    [Required, StringLength(20)] public string Sexo { get; set; } = "";
    [StringLength(30)] public string Telefono { get; set; } = "";
    [EmailAddress, StringLength(150)] public string Correo { get; set; } = "";
    [StringLength(250)] public string Direccion { get; set; } = "";
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    public ICollection<HistorialMedico> Historiales { get; set; } = new List<HistorialMedico>();
}

public class Especialidad : IEntity
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Nombre { get; set; } = "";
    public ICollection<Medico> Medicos { get; set; } = new List<Medico>();
}

public class Medico : IEntity
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Nombres { get; set; } = "";
    [Required, StringLength(100)] public string Apellidos { get; set; } = "";
    [EmailAddress, StringLength(150)] public string Correo { get; set; } = "";
    [StringLength(30)] public string Telefono { get; set; } = "";
    [Range(1, int.MaxValue)] public int EspecialidadId { get; set; }
    public Especialidad? Especialidad { get; set; }
    public ICollection<Cita> Citas { get; set; } = new List<Cita>();
}

public class Cita : IEntity
{
    public int Id { get; set; }
    [Range(1, int.MaxValue)] public int PacienteId { get; set; }
    public Paciente? Paciente { get; set; }
    [Range(1, int.MaxValue)] public int MedicoId { get; set; }
    public Medico? Medico { get; set; }
    public DateTime FechaHora { get; set; }
    [Required, StringLength(500)] public string Motivo { get; set; } = "";
    [Required, StringLength(30)] public string Estado { get; set; } = "Programada";
    public ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}

public class HistorialMedico : IEntity
{
    public int Id { get; set; }
    [Range(1, int.MaxValue)] public int PacienteId { get; set; }
    public Paciente? Paciente { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    [Required, StringLength(1000)] public string Diagnostico { get; set; } = "";
    [StringLength(3000)] public string Observaciones { get; set; } = "";
    [StringLength(2000)] public string Tratamiento { get; set; } = "";
}

public class Medicamento : IEntity
{
    public int Id { get; set; }
    [Required, StringLength(150)] public string Nombre { get; set; } = "";
    [StringLength(150)] public string Presentacion { get; set; } = "";
    [Range(0, 100000000)] public decimal Precio { get; set; }
    public ICollection<DetalleFactura> DetallesFactura { get; set; } = new List<DetalleFactura>();
}

public class Factura : IEntity
{
    public int Id { get; set; }
    [Range(1, int.MaxValue)] public int CitaId { get; set; }
    public Cita? Cita { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    [Range(0, 100000000)] public decimal Total { get; set; }
    [Required, StringLength(30)] public string Estado { get; set; } = "Pendiente";
    public ICollection<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
}

public class DetalleFactura : IEntity
{
    public int Id { get; set; }
    [Range(1, int.MaxValue)] public int FacturaId { get; set; }
    public Factura? Factura { get; set; }
    [Range(1, int.MaxValue)] public int MedicamentoId { get; set; }
    public Medicamento? Medicamento { get; set; }
    [Range(1, 100000)] public int Cantidad { get; set; }
    [Range(0, 100000000)] public decimal PrecioUnitario { get; set; }
}

public class SeguroMedico : IEntity
{
    public int Id { get; set; }
    [Required, StringLength(150)] public string Aseguradora { get; set; } = "";
    [Required, StringLength(100)] public string NumeroPoliza { get; set; } = "";
    public DateTime? VigenciaHasta { get; set; }
    [Range(1, int.MaxValue)] public int PacienteId { get; set; }
    public Paciente? Paciente { get; set; }
}

public class Usuario : IEntity
{
    public int Id { get; set; }
    [Required, StringLength(120)] public string Nombre { get; set; } = "";
    [Required, EmailAddress, StringLength(150)] public string Correo { get; set; } = "";
    [Required, StringLength(40)] public string Rol { get; set; } = "";
    [Required] public string PasswordHash { get; set; } = "";
    public bool Activo { get; set; } = true;
}
