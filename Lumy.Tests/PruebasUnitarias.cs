using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Lumy.Api.Data;
using Lumy.Api.Controllers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Lumy.Tests
{
    public class PruebasLibros
    {
        private ContextoLumy CrearContextoFalso(string nombreDb)
        {
            var opciones = new DbContextOptionsBuilder<ContextoLumy>()
                .UseInMemoryDatabase(databaseName: nombreDb)
                .Options;
            
            return new ContextoLumy(opciones);
        }

        [Fact]
        public async Task GuardarFavorito_DebeRetornarOk_CuandoEsNuevo()
        {
            var db = CrearContextoFalso("DbTest1");
            var controller = new LibrosController(db, null); 
            var nuevoLibro = new LibroDto { IdExterno = "OL123", Titulo = "Test Book", Autores = "Tester" };

            var resultado = await controller.GuardarFavorito(nuevoLibro);

            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task GuardarFavorito_DebeRetornarConflict_SiYaExiste()
        {
            var db = CrearContextoFalso("DbTest2");
            var controller = new LibrosController(db, null);
            
            db.Favoritos.Add(new LibroFavorito { UsuarioId = 1, IdExterno = "OL_REP", Titulo = "A", Autores = "B" });
            await db.SaveChangesAsync();

            var libroRepetido = new LibroDto { IdExterno = "OL_REP", Titulo = "A" };

            var resultado = await controller.GuardarFavorito(libroRepetido);

            Assert.IsType<ConflictObjectResult>(resultado);
        }

        [Fact]
        public async Task GuardarFavorito_DebeRetornarBadRequest_SiEsNull()
        {
            var db = CrearContextoFalso("DbTest3");
            var controller = new LibrosController(db, null);

            var resultado = await controller.GuardarFavorito(null);

            Assert.IsType<BadRequestObjectResult>(resultado);
        }

        [Fact]
        public async Task EliminarFavorito_DebeRetornarNotFound_SiNoExiste()
        {
            var db = CrearContextoFalso("DbTest4");
            var controller = new LibrosController(db, null);

            var resultado = await controller.EliminarFavorito(999); 

            Assert.IsType<NotFoundObjectResult>(resultado);
        }

        [Fact]
        public async Task GuardarFavorito_DebeGuardarDatosCorrectos()
        {
            var db = CrearContextoFalso("DbTest5");
            var controller = new LibrosController(db, null);
            var libro = new LibroDto { IdExterno = "ABC", Titulo = "El Principito", Autores = "Saint-Exupéry" };

            await controller.GuardarFavorito(libro);

            var guardado = await db.Favoritos.FirstOrDefaultAsync(f => f.IdExterno == "ABC");
            
            Assert.NotNull(guardado);
            Assert.Equal("El Principito", guardado.Titulo);
        }
    }
}