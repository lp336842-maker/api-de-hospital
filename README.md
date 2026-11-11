# Sistema de gestión de pacientes

Aplicación de gestión clínica desarrollada con ASP.NET Core Web API, React, Entity Framework Core y SQLite. La solución está separada en capas y utiliza interfaces, repositorios, servicios e inyección de dependencias.

## Estructura

- `ClinicaPacientes.Api`: controladores REST, configuración, Swagger y CORS.
- `ClinicaPacientes.Application`: DTO, interfaces y servicios de aplicación.
- `ClinicaPacientes.Domain`: entidades y contrato de entidad.
- `ClinicaPacientes.Infrastructure`: contexto de Entity Framework Core y repositorios.
- `ClinicaPacientes.Web`: interfaz React y Vite.

## Base de datos

La aplicación crea automáticamente `clinica.db` al iniciar la API. Contiene diez tablas: Pacientes, Especialidades, Medicos, Citas, HistorialesMedicos, Medicamentos, Facturas, DetallesFactura, SegurosMedicos y Usuarios. Se agregan especialidades iniciales cuando la tabla está vacía.

## Requisitos

- .NET 10 SDK
- Node.js 20 o superior
- npm

## Ejecutar la API

Desde la carpeta raíz:

```powershell
cd ClinicaPacientes.Api
dotnet restore
dotnet run --urls http://localhost:5130
```

Swagger estará disponible en `http://localhost:5130/swagger`.

## Ejecutar React

En otra terminal:

```powershell
cd ClinicaPacientes.Web
npm install
npm run dev
```

La aplicación estará disponible en `http://localhost:5173`.

Para cambiar la dirección de la API, crea un archivo `.env` dentro de `ClinicaPacientes.Web` con:

```text
VITE_API_URL=http://localhost:5130
```

## Módulos y endpoints

Todos los módulos tienen operaciones GET, GET por ID, POST, PUT y DELETE:

- `/api/Pacientes`
- `/api/Especialidades`
- `/api/Medicos`
- `/api/Citas`
- `/api/HistorialesMedicos`
- `/api/Medicamentos`
- `/api/Facturas`
- `/api/DetallesFactura`
- `/api/SegurosMedicos`
- `/api/Usuarios`

El endpoint `/api/Dashboard` devuelve los indicadores del panel principal. Los usuarios se devuelven sin exponer el hash de contraseña; las contraseñas nuevas se almacenan con BCrypt.

## Funcionalidades

- Dashboard con indicadores de pacientes, médicos, citas, medicamentos, facturas e ingresos.
- CRUD para las diez entidades.
- Búsqueda de pacientes y filtrado local en los demás módulos.
- Formularios con selección de relaciones entre tablas.
- Validación de campos obligatorios en la interfaz.
- Confirmación antes de eliminar registros.
- Documentación interactiva de la API con Swagger.
