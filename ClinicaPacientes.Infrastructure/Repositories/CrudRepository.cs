using ClinicaPacientes.Application.Interfaces;
using ClinicaPacientes.Domain.Entities;
using ClinicaPacientes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicaPacientes.Infrastructure.Repositories;

public class CrudRepository<T>(ClinicaDbContext db) : ICrudRepository<T> where T : class, IEntity
{
    public async Task<IReadOnlyList<T>> ObtenerTodosAsync(CancellationToken ct) => await db.Set<T>().AsNoTracking().ToListAsync(ct);
    public Task<T?> ObtenerPorIdAsync(int id, CancellationToken ct) => db.Set<T>().FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task AgregarAsync(T entidad, CancellationToken ct) => db.Set<T>().AddAsync(entidad, ct).AsTask();
    public Task GuardarCambiosAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
    public Task EliminarAsync(T entidad, CancellationToken ct)
    {
        db.Set<T>().Remove(entidad);
        return Task.CompletedTask;
    }
}
