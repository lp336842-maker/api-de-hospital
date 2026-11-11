using ClinicaPacientes.Application.Interfaces;
using ClinicaPacientes.Domain.Entities;

namespace ClinicaPacientes.Application.Services;

public class CrudService<T>(ICrudRepository<T> repository) : ICrudService<T> where T : class, IEntity
{
    public Task<IReadOnlyList<T>> ObtenerTodosAsync(CancellationToken ct) => repository.ObtenerTodosAsync(ct);
    public Task<T?> ObtenerPorIdAsync(int id, CancellationToken ct) => repository.ObtenerPorIdAsync(id, ct);
    public async Task<T> CrearAsync(T entidad, CancellationToken ct)
    {
        entidad.Id = 0;
        await repository.AgregarAsync(entidad, ct);
        await repository.GuardarCambiosAsync(ct);
        return entidad;
    }
    public async Task<T?> ActualizarAsync(int id, T entidad, CancellationToken ct)
    {
        var actual = await repository.ObtenerPorIdAsync(id, ct);
        if (actual is null) return null;
        var tipo = typeof(T);
        foreach (var propiedad in tipo.GetProperties().Where(p => p.CanRead && p.CanWrite && p.Name != nameof(IEntity.Id) && propiedadEscalar(p.PropertyType)))
            propiedad.SetValue(actual, propiedad.GetValue(entidad));
        await repository.GuardarCambiosAsync(ct);
        return actual;
    }
    public async Task<bool> EliminarAsync(int id, CancellationToken ct)
    {
        var entidad = await repository.ObtenerPorIdAsync(id, ct);
        if (entidad is null) return false;
        await repository.EliminarAsync(entidad, ct);
        await repository.GuardarCambiosAsync(ct);
        return true;
    }
    private static bool propiedadEscalar(Type type) => type.IsValueType || type == typeof(string);
}
