using Lumy.Api.Data;
using Lumy.Api.Servicios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ContextoLumy>(opc => 
    opc.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<IServicioLibros, ServicioLibros>();
builder.Services.AddControllers();

builder.Services.AddCors(opc => opc.AddPolicy("PermitirAngular", p => 
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseCors("PermitirAngular");
app.MapControllers();
app.Run();