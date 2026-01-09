using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lumy.Api.Data;
using Lumy.Api.Servicios;

namespace Lumy.Api.Controllers;

[ApiController]
[Route("api")]
public class LibrosController : ControllerBase
{
    private readonly ContextoLumy _bd;
    private readonly IServicioLibros _servicio;

    public LibrosController(ContextoLumy bd, IServicioLibros servicio)
    {
        _bd = bd;
        _servicio = servicio;
    }

    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return BadRequest("Escribe algo para buscar.");
        var resultados = await _servicio.BuscarEnApiExterna(q);
        return Ok(resultados);
    }

    [HttpGet("favoritos")]
    public async Task<IActionResult> ObtenerFavoritos()
    {
        var lista = await _bd.Favoritos.Where(f => f.UsuarioId == 1).ToListAsync();
        return Ok(lista);
    }

    [HttpPost("favoritos")]
    public async Task<IActionResult> GuardarFavorito([FromBody] LibroDto libro)
    {
        if (libro == null) return BadRequest("Datos inválidos");

        bool existe = await _bd.Favoritos.AnyAsync(f => f.UsuarioId == 1 && f.IdExterno == libro.IdExterno);
        
        if (existe) 
            return Conflict("¡Este libro ya está en tus favoritos de Lumy!");
            
        var nuevo = new LibroFavorito
        {
            UsuarioId = 1,
            IdExterno = libro.IdExterno,
            Titulo = libro.Titulo,
            Autores = libro.Autores,
            AnioPublicacion = libro.AnioPublicacion
        };

        _bd.Favoritos.Add(nuevo);
        await _bd.SaveChangesAsync();
        return Ok(nuevo);
    }

    // --- SOLO DEBE HABER UN MÉTODO PARA BORRAR ---
    
    [HttpDelete("favoritos/{id}")]
    public async Task<IActionResult> EliminarFavorito(int id)
    {
        var libro = await _bd.Favoritos.FindAsync(id); 

        if (libro == null)
        {
            return NotFound("El libro no existe");
        }

        _bd.Favoritos.Remove(libro);
        await _bd.SaveChangesAsync();

        return Ok("Libro eliminado correctamente");
    }

} // <--- Fin de la clase