using Lumy.Api.Data;
using Lumy.Api.Servicios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Base de Datos
builder.Services.AddDbContext<ContextoLumy>(opc => 
    opc.UseSqlServer(builder.Configuration.GetConnectionString("ConexionPorDefecto")));

// 2. Inyección de Dependencias
builder.Services.AddHttpClient<IServicioLibros, ServicioLibros>();
builder.Services.AddControllers();

// 3. CORS (Para Angular)
builder.Services.AddCors(opc => opc.AddPolicy("PermitirAngular", p => 
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseCors("PermitirAngular");
app.MapControllers();
app.Run();