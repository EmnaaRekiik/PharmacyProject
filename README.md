# Pharmacy Project

A full-stack pharmacy management system built with ASP.NET Core, Entity Framework Core, SQL Server, and Blazor WebAssembly.

The application includes:
- patient management
- pharmacist management
- medication catalog and stock tracking
- prescription creation and management
- dispensed medication tracking
- authentication and JWT-based login for the API
- a Blazor frontend for interacting with the system

## Project structure

- `API/` â€” ASP.NET Core Web API
- `FRONT/` â€” Blazor WebAssembly client
- `pharmacy/` â€” shared domain models and core entities
- `PharmacyProject.sln` â€” solution file

## Technologies

- .NET 9
- ASP.NET Core Web API
- Blazor WebAssembly
- Entity Framework Core
- SQL Server (LocalDB by default)
- ASP.NET Core Identity
- JWT Authentication
- Swagger / OpenAPI

## Prerequisites

Before running the project, make sure you have:

- .NET 9 SDK installed
- SQL Server LocalDB installed (the default connection string targets LocalDB)
- Visual Studio 2022 or VS Code with C# support (optional but recommended)

## Configuration

The API connection string is defined in:

- `API/appsettings.json`

Default connection string:

```json
"ConnectionStrings": {
  "cnx": "server=(localdb)\\MSSQLLocalDB;database=PharmacyDB;integrated security=true;trust server certificate=yes;"
}
```

The JWT configuration is also stored there, including the signing key used to generate access tokens.

## Restore dependencies

From the repository root:

```bash
dotnet restore
```

## Database setup

If the database does not already exist, create it and apply migrations:

```bash
dotnet ef database update --project API/API.csproj
```

If `dotnet ef` is not available, install the EF Core tool:

```bash
dotnet tool install --global dotnet-ef
```

## Run the backend

Start the API:

```bash
dotnet run --project API/API.csproj
```

The API runs by default on:

- http://localhost:5282

Swagger is enabled in development mode and can be accessed at:

- http://localhost:5282/swagger

## Run the frontend

In another terminal, start the Blazor frontend:

```bash
dotnet run --project FRONT/FRONT.csproj
```

The frontend runs by default on:

- http://localhost:5057

## Main API areas

The API includes controllers for:

- `AccountController` â€” registration and login
- `AdminController` â€” admin operations
- `PatientController` â€” patient management
- `PharmacistController` â€” pharmacist management
- `MedicationController` â€” medication catalog
- `PrescriptionController` â€” prescription handling
- `PrescriptionMedicationController` â€” prescription-medication links
- `DispensedMedicationController` â€” dispensed medication management

## Authentication

The API uses ASP.NET Core Identity together with JWT authentication.

Authentication flow:
1. Register or log in through the account endpoints.
2. Receive a JWT token from the login response.
3. Include the token in the `Authorization` header for protected requests.

Example:

```http
Authorization: Bearer <your-token>
```

## Notes

- The API is configured with CORS to allow the frontend to communicate with it during development.
- The app uses LocalDB by default; if you prefer another SQL Server instance, update the connection string in `API/appsettings.json`.
- The database model includes relationships between patients, prescriptions, pharmacists, medications, and dispensed medications.

## License

This project is for academic and learning purposes unless otherwise specified by the project owner.

