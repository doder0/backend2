# Task Management API

Portfolio-style ASP.NET Core Web API for task and to-do management.

## Tech Stack

- .NET 8
- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger / OpenAPI
- xUnit

## Run

```powershell
dotnet restore
dotnet run --project src/TaskManagementApi.Api
```

## Migrations

The app applies migrations on startup. To add or update migrations manually:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add NewMigrationName --project src/TaskManagementApi.Api --startup-project src/TaskManagementApi.Api
dotnet ef database update --project src/TaskManagementApi.Api --startup-project src/TaskManagementApi.Api
```

## Tests

```powershell
dotnet test
```

## Example Requests

Create a task:

```http
POST /api/tasks
Content-Type: application/json

{
  "title": "Finish backend portfolio",
  "description": "Complete API and tests",
  "status": "Open",
  "priority": "High",
  "dueDate": "2026-04-10T12:00:00Z"
}
```

Filter tasks:

```http
GET /api/tasks?status=Open&priority=High&dueBefore=2026-04-10T00:00:00Z&sortBy=dueDate&sortOrder=asc
```

Stats:

```http
GET /api/tasks/stats
```
