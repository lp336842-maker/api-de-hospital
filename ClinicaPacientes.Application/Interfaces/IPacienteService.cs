using ClinicaPacientes.Application.DTOs;
namespace ClinicaPacientes.Application.Interfaces;
public interface IPacienteService { Task<IReadOnlyList<PacienteDto>> ObtenerTodosAsync(string? busqueda,CancellationToken ct); Task<PacienteDto?> ObtenerPorIdAsync(int id,CancellationToken ct); Task<PacienteDto> CrearAsync(PacienteCrearDto dto,CancellationToken ct); Task<PacienteDto?> ActualizarAsync(int id,PacienteCrearDto dto,CancellationToken ct); Task<bool> EliminarAsync(int id,CancellationToken ct); }
