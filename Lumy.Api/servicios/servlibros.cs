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
public class RespuestaOpenLibrary 
{ 

    public List<DocExterno> Docs { get; set; } = new(); 
}

public class DocExterno 
{ 
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<string> Author_Name { get; set; } = new();
    public int? First_Publish_Year { get; set; } 
}