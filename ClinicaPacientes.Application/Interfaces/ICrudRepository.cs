using ClinicaPacientes.Domain.Entities;

namespace ClinicaPacientes.Application.Interfaces;

public interface ICrudRepository<T> where T : class, IEntity
{
    Task<IReadOnlyList<T>> ObtenerTodosAsync(CancellationToken ct);
    Task<T?> ObtenerPorIdAsync(int id, CancellationToken ct);
    Task AgregarAsync(T entidad, CancellationToken ct);
    Task GuardarCambiosAsync(CancellationToken ct);
    Task EliminarAsync(T entidad, CancellationToken ct);
}
