using ClinicaPacientes.Domain.Entities;

namespace ClinicaPacientes.Application.Interfaces;

public interface ICrudService<T> where T : class, IEntity
{
    Task<IReadOnlyList<T>> ObtenerTodosAsync(CancellationToken ct);
    Task<T?> ObtenerPorIdAsync(int id, CancellationToken ct);
    Task<T> CrearAsync(T entidad, CancellationToken ct);
    Task<T?> ActualizarAsync(int id, T entidad, CancellationToken ct);
    Task<bool> EliminarAsync(int id, CancellationToken ct);
}
