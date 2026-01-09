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
        // Método auxiliar para crear una Base de Datos "falsa" en memoria RAM
        private ContextoLumy CrearContextoFalso(string nombreDb)
        {
            var opciones = new DbContextOptionsBuilder<ContextoLumy>()
                .UseInMemoryDatabase(databaseName: nombreDb)
                .Options;
            
            return new ContextoLumy(opciones);
        }

        // TEST 1: Verificar que guarda correctamente (Caso Feliz)
        [Fact]
        public async Task GuardarFavorito_DebeRetornarOk_CuandoEsNuevo()
        {
            // Arrange (Preparar)
            var db = CrearContextoFalso("DbTest1");
            // Nota: Pasamos null en el servicio porque este test solo prueba la base de datos
            var controller = new LibrosController(db, null); 
            var nuevoLibro = new LibroDto { IdExterno = "OL123", Titulo = "Test Book", Autores = "Tester" };

            // Act (Actuar)
            var resultado = await controller.GuardarFavorito(nuevoLibro);

            // Assert (Verificar)
            Assert.IsType<OkObjectResult>(resultado); // Esperamos un 200 OK
        }

        // TEST 2: Evitar duplicados (Requisito obligatorio)
        [Fact]
        public async Task GuardarFavorito_DebeRetornarConflict_SiYaExiste()
        {
            // Arrange
            var db = CrearContextoFalso("DbTest2");
            var controller = new LibrosController(db, null);
            
            // Insertamos uno manualmente primero
            db.Favoritos.Add(new LibroFavorito { UsuarioId = 1, IdExterno = "OL_REP", Titulo = "A", Autores = "B" });
            await db.SaveChangesAsync();

            // Intentamos guardar el MISMO
            var libroRepetido = new LibroDto { IdExterno = "OL_REP", Titulo = "A" };

            // Act
            var resultado = await controller.GuardarFavorito(libroRepetido);

            // Assert
            Assert.IsType<ConflictObjectResult>(resultado); // Esperamos un 409 Conflict
        }

        // TEST 3: Validación de Request Inválido (Faltan campos)
        [Fact]
        public async Task GuardarFavorito_DebeRetornarBadRequest_SiEsNull()
        {
            // Arrange
            var db = CrearContextoFalso("DbTest3");
            var controller = new LibrosController(db, null);

            // Act
            var resultado = await controller.GuardarFavorito(null); // Enviamos null

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado); // Esperamos 400 Bad Request
        }

        // TEST 4: Eliminar favorito inexistente
        [Fact]
        public async Task EliminarFavorito_DebeRetornarNotFound_SiNoExiste()
        {
            // Arrange
            var db = CrearContextoFalso("DbTest4");
            var controller = new LibrosController(db, null);

            // Act
            // Intentamos borrar el ID 999 que no existe
            var resultado = await controller.EliminarFavorito(999); 

            // Assert
            Assert.IsType<NotFoundObjectResult>(resultado); // Esperamos 404 Not Found
        }

        // TEST 5: Verificar que se guarda con los datos correctos (Mapeo)
        [Fact]
        public async Task GuardarFavorito_DebeGuardarDatosCorrectos()
        {
            // Arrange
            var db = CrearContextoFalso("DbTest5");
            var controller = new LibrosController(db, null);
            var libro = new LibroDto { IdExterno = "ABC", Titulo = "El Principito", Autores = "Saint-Exupéry" };

            // Act
            await controller.GuardarFavorito(libro);

            // Assert - Buscamos en la DB falsa a ver si está ahí
            var guardado = await db.Favoritos.FirstOrDefaultAsync(f => f.IdExterno == "ABC");
            
            Assert.NotNull(guardado);
            Assert.Equal("El Principito", guardado.Titulo); // Verificamos que el título coincida
        }
    }
}