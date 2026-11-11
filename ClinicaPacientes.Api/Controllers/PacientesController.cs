using ClinicaPacientes.Application.DTOs;using ClinicaPacientes.Application.Interfaces;using Microsoft.AspNetCore.Mvc;
namespace ClinicaPacientes.Api.Controllers;
[ApiController][Route("api/[controller]")]public class PacientesController(IPacienteService service):ControllerBase{
[HttpGet]public async Task<ActionResult<IReadOnlyList<PacienteDto>>> Obtener([FromQuery]string? busqueda,CancellationToken ct)=>Ok(await service.ObtenerTodosAsync(busqueda,ct));
[HttpGet("{id:int}")]public async Task<ActionResult<PacienteDto>> ObtenerPorId(int id,CancellationToken ct){var p=await service.ObtenerPorIdAsync(id,ct);return p is null?NotFound():Ok(p);}
[HttpPost]public async Task<ActionResult<PacienteDto>> Crear([FromBody]PacienteCrearDto dto,CancellationToken ct){try{var p=await service.CrearAsync(dto,ct);return CreatedAtAction(nameof(ObtenerPorId),new{id=p.Id},p);}catch(InvalidOperationException e){return Conflict(new{mensaje=e.Message});}}
[HttpPut("{id:int}")]public async Task<ActionResult<PacienteDto>> Actualizar(int id,[FromBody]PacienteCrearDto dto,CancellationToken ct){try{var p=await service.ActualizarAsync(id,dto,ct);return p is null?NotFound():Ok(p);}catch(InvalidOperationException e){return Conflict(new{mensaje=e.Message});}}
[HttpDelete("{id:int}")]public async Task<IActionResult> Eliminar(int id,CancellationToken ct)=>await service.EliminarAsync(id,ct)?NoContent():NotFound();}
