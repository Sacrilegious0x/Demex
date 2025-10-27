using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Moq; 
using Microsoft.EntityFrameworkCore.Infrastructure;
using LAFABRICA.Data.DB; 
using LAFABRICA.Models; 
using LAFABRICA.Services;

namespace LAFABRICA.Service.Tests
{
    public class MaterialServiceTests
    {

        // 1. Método de Configuración para el Contexto In-Memory

        private AppDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new AppDbContext(options);
        }

        // 2. Pruebas Unitarias para CreateMaterial
        [Fact]
        public async Task CreateMaterial_ShouldCreateThreeRecords_WhenCalledWithValidData()
        {
            // ARRANGE (Preparación): Configurar el entorno y los datos de prueba
            string dbName = "CreateMaterialTestDB1";

            // 1. Crear el contexto In-Memory
            var testContext = GetInMemoryDbContext(dbName);

            // 2. Crear un Mock de IDbContextFactory
            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();

            // 3. Configurar el Mock: cuando alguien llame a CreateDbContext(), debe devolver nuestro contexto In-Memory
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);

            // 4. Inicializar el servicio, pasando el Mock de la factoría (.Object)
            var service = new MaterialService(mockFactory.Object);

            // Datos de entrada para la prueba
            var newMaterial = new Material
            {
                Name = "Tornillo M8",
                Unit = "Unidad",
                PricePurchase = 0.50m,
                PhotoUrl = "default"
            };
            int supplierId = 101;
            int? quantityToMaterialSupplier = 500;
            int? minimumQuantity = 50;

            // ACT (Ejecución): Llamar al método a probar
            var createdMaterial = await service.CreateMaterial(
                newMaterial,
                supplierId,
                quantityToMaterialSupplier,
                minimumQuantity
            );

            // ASSERT (Verificación): Usar el mismo contexto In-Memory (que ya contiene los datos)
            // Ya que el contexto fue creado por el Mock, lo usamos directamente para verificar.
            var material = await testContext.Materials.FirstOrDefaultAsync(m => m.Name == "Tornillo M8");
            Assert.NotNull(material);
            Assert.Equal("Unidad", material.Unit);

            var materialSupplier = await testContext.MaterialSuppliers
                .FirstOrDefaultAsync(ms => ms.MaterialId == material.Id && ms.SupplierId == supplierId);
            Assert.NotNull(materialSupplier);
            Assert.Equal(500, materialSupplier.Quantity);

            var inventory = await testContext.Inventories
                .FirstOrDefaultAsync(i => i.MaterialId == material.Id);
            Assert.NotNull(inventory);
            Assert.Equal(50, inventory.Quantity);

            // IMPORTANTE: El contexto In-Memory debe ser cerrado o liberado después de la prueba.
            // Esto se maneja mejor usando un IDisposable si se usa la misma DB en múltiples pruebas, 
            // pero para esta prueba unitaria simple, el contexto es desechable al final del método.
        }

        [Fact]
        public async Task CreateMaterial_ShouldHandleNullPhotoUrlAndSetDefault()
        {
            // ARRANGE
            string dbName = "CreateMaterialTestDB2";

            var testContext = GetInMemoryDbContext(dbName);
            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);
            var service = new MaterialService(mockFactory.Object);

            var newMaterial = new Material
            {
                Name = "Alambre Cobre",
                Unit = "Metro",
                PricePurchase = 1.20m,
                PhotoUrl = null
            };
            int supplierId = 202;
            int? quantityToMaterialSupplier = 100;
            int? minimumQuantity = 10;

            // ACT
            var createdMaterial = await service.CreateMaterial(
                newMaterial,
                supplierId,
                quantityToMaterialSupplier,
                minimumQuantity
            );

            // ASSERT
            var material = await testContext.Materials.FirstOrDefaultAsync(m => m.Name == "Alambre Cobre");
            Assert.NotNull(material);
            // Verifica que la lógica dentro del servicio funcione
            Assert.Equal("default", material.PhotoUrl);
        }
    }
}
