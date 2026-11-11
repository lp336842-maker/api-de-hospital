using ClinicaPacientes.Domain.Entities;
namespace ClinicaPacientes.Application.Interfaces;
public interface IPacienteRepository { Task<IReadOnlyList<Paciente>> ObtenerTodosAsync(string? busqueda,CancellationToken ct); Task<Paciente?> ObtenerPorIdAsync(int id,CancellationToken ct); Task<bool> ExisteDocumentoAsync(string documento,int? excluirId,CancellationToken ct); Task AgregarAsync(Paciente paciente,CancellationToken ct); Task GuardarCambiosAsync(CancellationToken ct); Task EliminarAsync(Paciente paciente,CancellationToken ct); }
