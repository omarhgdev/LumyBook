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
-- 1. Crear bd

IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'LumyDB')
BEGIN
    CREATE DATABASE LumyDB;
END
GO

USE LumyDB;
GO

-- 2. Tabla Usuarios
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND type in (N'U'))
BEGIN
    CREATE TABLE Usuarios (
        Id INT PRIMARY KEY IDENTITY(1,1),
        NombreUsuario NVARCHAR(50) NOT NULL UNIQUE
    );
END
GO

-- 3. Tabla Favoritos
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Favoritos]') AND type in (N'U'))
BEGIN
    CREATE TABLE Favoritos (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UsuarioId INT NOT NULL,
        IdExterno NVARCHAR(100) NOT NULL,
        Titulo NVARCHAR(255) NOT NULL,
        Autores NVARCHAR(255) NOT NULL,
        AnioPublicacion INT NULL,
        FechaCreacion DATETIME DEFAULT GETDATE(),
        
        CONSTRAINT FK_Favoritos_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
    );
END
GO

-- 4. Evitar duplicados
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuario_IdExterno' AND object_id = OBJECT_ID('Favoritos'))
BEGIN
    CREATE UNIQUE INDEX IX_Usuario_IdExterno ON Favoritos(UsuarioId, IdExterno);
END
GO

-- 5. Usuario prueba
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE NombreUsuario = 'usuario_prueba')
BEGIN
    INSERT INTO Usuarios (NombreUsuario) VALUES ('usuario_prueba');
END
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
