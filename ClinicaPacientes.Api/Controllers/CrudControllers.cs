using ClinicaPacientes.Application.Interfaces;
using ClinicaPacientes.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicaPacientes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class CrudController<T>(ICrudService<T> service) : ControllerBase where T : class, IEntity
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<T>>> Obtener(CancellationToken ct) => Ok(await service.ObtenerTodosAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<T>> ObtenerPorId(int id, CancellationToken ct)
    {
        var entidad = await service.ObtenerPorIdAsync(id, ct);
        return entidad is null ? NotFound() : Ok(entidad);
    }

    [HttpPost]
    public async Task<ActionResult<T>> Crear([FromBody] T entidad, CancellationToken ct)
    {
        var creada = await service.CrearAsync(entidad, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, creada);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<T>> Actualizar(int id, [FromBody] T entidad, CancellationToken ct)
    {
        var actualizada = await service.ActualizarAsync(id, entidad, ct);
        return actualizada is null ? NotFound() : Ok(actualizada);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct) => await service.EliminarAsync(id, ct) ? NoContent() : NotFound();
}

public class EspecialidadesController(ICrudService<Especialidad> service) : CrudController<Especialidad>(service) { }
public class MedicosController(ICrudService<Medico> service) : CrudController<Medico>(service) { }
public class CitasController(ICrudService<Cita> service) : CrudController<Cita>(service) { }
public class HistorialesMedicosController(ICrudService<HistorialMedico> service) : CrudController<HistorialMedico>(service) { }
public class MedicamentosController(ICrudService<Medicamento> service) : CrudController<Medicamento>(service) { }
public class FacturasController(ICrudService<Factura> service) : CrudController<Factura>(service) { }
public class DetallesFacturaController(ICrudService<DetalleFactura> service) : CrudController<DetalleFactura>(service) { }
public class SegurosMedicosController(ICrudService<SeguroMedico> service) : CrudController<SeguroMedico>(service) { }

public record UsuarioDto(int Id, string Nombre, string Correo, string Rol, bool Activo);
public record UsuarioCrearDto(string Nombre, string Correo, string Rol, string? Password, bool Activo);

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(ICrudService<Usuario> service) : ControllerBase
{
    private static UsuarioDto Map(Usuario u) => new(u.Id, u.Nombre, u.Correo, u.Rol, u.Activo);

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UsuarioDto>>> Obtener(CancellationToken ct) => Ok((await service.ObtenerTodosAsync(ct)).Select(Map));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> ObtenerPorId(int id, CancellationToken ct)
    {
        var usuario = await service.ObtenerPorIdAsync(id, ct);
        return usuario is null ? NotFound() : Ok(Map(usuario));
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Crear(UsuarioCrearDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Password)) return BadRequest(new { mensaje = "La contraseña es obligatoria." });
        var usuario = new Usuario { Nombre = dto.Nombre.Trim(), Correo = dto.Correo.Trim(), Rol = dto.Rol, PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), Activo = dto.Activo };
        var creado = await service.CrearAsync(usuario, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, Map(creado));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> Actualizar(int id, UsuarioCrearDto dto, CancellationToken ct)
    {
        var actual = await service.ObtenerPorIdAsync(id, ct);
        if (actual is null) return NotFound();
        actual.Nombre = dto.Nombre.Trim();
        actual.Correo = dto.Correo.Trim();
        actual.Rol = dto.Rol;
        actual.Activo = dto.Activo;
        if (!string.IsNullOrWhiteSpace(dto.Password)) actual.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var actualizado = await service.ActualizarAsync(id, actual, ct);
        return actualizado is null ? NotFound() : Ok(Map(actualizado));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct) => await service.EliminarAsync(id, ct) ? NoContent() : NotFound();
}

[ApiController]
[Route("api/[controller]")]
public class DashboardController(ClinicaPacientes.Infrastructure.Data.ClinicaDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Obtener(CancellationToken ct)
    {
        return Ok(new
        {
            pacientes = await db.Pacientes.CountAsync(ct),
            medicos = await db.Medicos.CountAsync(ct),
            citas = await db.Citas.CountAsync(ct),
            citasPendientes = await db.Citas.CountAsync(x => x.Estado == "Programada", ct),
            medicamentos = await db.Medicamentos.CountAsync(ct),
            facturasPendientes = await db.Facturas.CountAsync(x => x.Estado == "Pendiente", ct),
            ingresos = await db.Facturas.Where(x => x.Estado == "Pagada").SumAsync(x => (decimal?)x.Total, ct) ?? 0
        });
    }
}
