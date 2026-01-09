using Microsoft.EntityFrameworkCore;

namespace Lumy.Api.Data;

public class LibroDto
{
    public string IdExterno { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Autores { get; set; } = string.Empty;
    public int? AnioPublicacion { get; set; }
}

public class LibroFavorito
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string IdExterno { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Autores { get; set; } = string.Empty;
    public int? AnioPublicacion { get; set; }
}

public class ContextoLumy : DbContext
{
    public ContextoLumy(DbContextOptions<ContextoLumy> opciones) : base(opciones) { }
    public DbSet<LibroFavorito> Favoritos { get; set; }
}