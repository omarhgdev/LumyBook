# LumyBook

## Usado


* **Backend:** .NET 10 SDK
* **Framework:** ASP.NET Core Web API.
* **Base de Datos:** SQL Server Express.
* **Testing:** xUnit + Entity Framework Core In-Memory.

* **Angular 19+**
* **Node.js v20+**
---

### 1. Ejecutar DDL
```sql
CREATE DATABASE LumyDB;
GO

USE LumyDB;
GO

CREATE TABLE Favoritos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,        -- Simulación de usuario (por defecto 1)
    IdExterno NVARCHAR(100) NOT NULL,
    Titulo NVARCHAR(255) NOT NULL,
    Autores NVARCHAR(255),
    AnioPublicacion INT
);
GO
```
### 2. Connection String
Lumy.Api > appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LumyDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "AllowedHosts": "*"
}
```

### 3. Ejecutar Proyecto
```bash
cd Lumy.Api
dotnet run
```
Angular
```bash
cd lumy-cliente
npm install
ng serve
```
### 4. Ejecutar Tests
```bash
dotnet test
```
