using Lumy.Api.Data;

namespace Lumy.Api.Servicios;

public interface IServicioLibros {
    Task<List<LibroDto>> BuscarEnApiExterna(string busqueda);
}

public class ServicioLibros : IServicioLibros
{
    private readonly HttpClient _http;

    public ServicioLibros(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<LibroDto>> BuscarEnApiExterna(string busqueda)
    {
        try 
        {
            var url = $"https://openlibrary.org/search.json?q={busqueda}&fields=key,title,author_name,first_publish_year&limit=10";
            var respuesta = await _http.GetFromJsonAsync<RespuestaOpenLibrary>(url);

            if (respuesta?.Docs == null) return new List<LibroDto>();

            // Convertimos la respuesta en inglés a nuestro modelo Lumy en español
            return respuesta.Docs.Select(doc => new LibroDto
            {
                IdExterno = doc.Key,
                Titulo = doc.Title,
                Autores = doc.Author_Name != null ? string.Join(", ", doc.Author_Name) : "Desconocido",
                AnioPublicacion = doc.First_Publish_Year
            }).ToList();
        }
        catch 
        {
            return new List<LibroDto>();
        }
    }
}

// Clases auxiliares para leer el JSON externo
// Clases auxiliares para leer el JSON externo
public class RespuestaOpenLibrary 
{ 
    // Solución: Le decimos "= new();" para que nazca como una lista vacía, no nula.
    public List<DocExterno> Docs { get; set; } = new(); 
}

public class DocExterno 
{ 
    // Solución: Le decimos "= string.Empty;" para que nazca como texto vacío.
    public string Key { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    // Solución: Lista vacía por defecto
    public List<string> Author_Name { get; set; } = new();
    
    // Este no da error porque tiene el "?" (int?), lo que significa que LE PERMITES ser nulo.
    public int? First_Publish_Year { get; set; } 
}