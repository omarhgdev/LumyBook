IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'LumyDB')
BEGIN
    CREATE DATABASE LumyDB;
END
GO

USE LumyDB;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Favoritos]') AND type in (N'U'))
BEGIN
    CREATE TABLE Favoritos (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId INT NOT NULL,
        IdExterno NVARCHAR(100) NOT NULL,
        Titulo NVARCHAR(255) NOT NULL,
        Autores NVARCHAR(255),
        AnioPublicacion INT
    );
END
GO