using System.Text.Json.Serialization;
using ClinicaPacientes.Application.Interfaces;
using ClinicaPacientes.Application.Services;
using ClinicaPacientes.Domain.Entities;
using ClinicaPacientes.Infrastructure.Data;
using ClinicaPacientes.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ClinicaDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Clinica") ?? "Data Source=clinica.db"));
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IPacienteService, PacienteService>();

builder.Services.AddScoped(typeof(ICrudRepository<>), typeof(CrudRepository<>));
builder.Services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>));

builder.Services.AddCors(options => options.AddPolicy("spa", policy => policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173", "http://localhost:4173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClinicaDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (!await db.Especialidades.AnyAsync())
    {
        db.Especialidades.AddRange(
            new Especialidad { Nombre = "Medicina General" },
            new Especialidad { Nombre = "Pediatría" },
            new Especialidad { Nombre = "Cardiología" },
            new Especialidad { Nombre = "Dermatología" },
            new Especialidad { Nombre = "Ginecología" });
        await db.SaveChangesAsync();
    }
}

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (DbUpdateException)
    {
        if (context.Response.HasStarted) throw;
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        await context.Response.WriteAsJsonAsync(new { mensaje = "No se pudo completar la operación porque el registro está relacionado con otros datos o existe información duplicada." });
    }
    catch (InvalidOperationException exception)
    {
        if (context.Response.HasStarted) throw;
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { mensaje = exception.Message });
    }
});

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ClinicaPacientes API v1");
    options.RoutePrefix = "swagger";
});
app.UseCors("spa");
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger/index.html"));
app.MapGet("/swagger", () => Results.Redirect("/swagger/index.html"));
app.Run();
