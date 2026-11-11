namespace ClinicaPacientes.Application.DTOs;
public record PacienteDto(int Id,string Nombres,string Apellidos,string Documento,DateTime FechaNacimiento,string Sexo,string Telefono,string Correo,string Direccion,DateTime FechaRegistro);
public record PacienteCrearDto(string Nombres,string Apellidos,string Documento,DateTime FechaNacimiento,string Sexo,string Telefono,string Correo,string Direccion);
