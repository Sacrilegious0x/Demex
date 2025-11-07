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
        // Método de Configuración que configura la BD en la memoria al parecer
        private AppDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new AppDbContext(options);
        }

        // Pruebas Unitarias para CreateMaterial (versión corregida)
        [Fact]
        public async Task CreateMaterial_ShouldCreateThreeRecords_WhenCalledWithValidData()
        {
            // ARRANGE
            string dbName = "CreateMaterialTestDB1";


            var testContext = GetInMemoryDbContext(dbName);

            // Mock de la factoría: devuelve una NUEVA instancia en cada llamada.
            // Devolver nuevas instancias permite que el servicio cree y disponga su propio contexto
            // sin afectar a 'testContext' que usamos para comprobar resultsss.
            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext())
                       .Returns(() => GetInMemoryDbContext(dbName)); // lambda que crea una instancia nueva

            var service = new MaterialService(mockFactory.Object);

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

            // ACT
            var createdMaterial = await service.CreateMaterial(
                newMaterial,
                supplierId,
                quantityToMaterialSupplier,
                minimumQuantity
            );

            // ASSERT: se usa 'testContext' (que NO fue devuelto al servicio) para leer lo que se cambio, asi lo entendi.
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
            Assert.Equal(minimumQuantity, inventory.MinimunQuantity);

            // Limpieza 
            await testContext.Database.EnsureDeletedAsync();
            await testContext.DisposeAsync();
        }

        [Fact]
        public async Task CreateMaterial_ShouldHandleNullPhotoUrlAndSetDefault()
        {
            
            string dbName = "CreateMaterialTestDB2";

            
            var testContext = GetInMemoryDbContext(dbName);

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext())
                       .Returns(() => GetInMemoryDbContext(dbName)); 

            var service = new MaterialService(mockFactory.Object);

            var newMaterial = new Material
            {
                Name = "Alambre Cobre",
                Unit = "Metro",
                PricePurchase = 100,
                PhotoUrl = null
            };
            int supplierId = 202;
            int? quantityToMaterialSupplier = 100;
            int? minimumQuantity = 10;

            
            var createdMaterial = await service.CreateMaterial(
                newMaterial,
                supplierId,
                quantityToMaterialSupplier,
                minimumQuantity
            );

            
            var material = await testContext.Materials.FirstOrDefaultAsync(m => m.Name == "Alambre Cobre");
            Assert.NotNull(material);
            // Verifica que la lógica dentro del servicio haya puesto "default" si PhotoUrl venía null
            Assert.Equal("default", material.PhotoUrl);

           
            await testContext.Database.EnsureDeletedAsync();
            await testContext.DisposeAsync();
        }

        [Fact]
        public async Task GetMaterialById_ShouldReturnMaterial_WhenExists()
        {
            
            string dbName = Guid.NewGuid().ToString(); 
            var seedContext = GetInMemoryDbContext(dbName);
            var seededMaterial = new Material
            {
                Name = "Botones Verdes",
                Unit = "Unidad",
                PricePurchase = 100,
                PhotoUrl = "default"
            };
            seedContext.Materials.Add(seededMaterial);
            await seedContext.SaveChangesAsync();

            var insertedId = seededMaterial.Id; 
            await seedContext.DisposeAsync(); 
            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext())
                       .Returns(() => GetInMemoryDbContext(dbName));
            var service = new MaterialService(mockFactory.Object);
            var result = await service.GetMaterialById(insertedId);
            

            Assert.NotNull(result);
            Assert.Equal(insertedId, result.Id);
            Assert.Equal("Botones Verdes", result.Name);
        }

        [Fact]
        public async Task DeleteMaterial_ShouldSetIsActiveToFalse_WhenMaterialExists()
        {
            string dbName = Guid.NewGuid().ToString();

            var seedContext = GetInMemoryDbContext(dbName);
            var seededMaterial = new Material
            {
                Name = "Arandela 10",
                Unit = "Unidad",
                PricePurchase = 100,
                PhotoUrl = "default",
                IsActive = true 
            };
            seedContext.Materials.Add(seededMaterial);
            await seedContext.SaveChangesAsync();
            var insertedId = seededMaterial.Id;
            await seedContext.DisposeAsync();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext())
                       .Returns(() => GetInMemoryDbContext(dbName));

            var service = new MaterialService(mockFactory.Object);

            await service.DeleteMaterial(insertedId);

            var verifyContext = GetInMemoryDbContext(dbName);
            var materialAfter = await verifyContext.Materials.FindAsync(insertedId);
            Assert.NotNull(materialAfter);
            Assert.False(materialAfter.IsActive); 

            await verifyContext.Database.EnsureDeletedAsync();
            await verifyContext.DisposeAsync();
        }

        [Fact]
        public async Task UpdateMaterial_ShouldUpdateFields_WhenMaterialExists()
        {
            string dbName = Guid.NewGuid().ToString();
            var seedContext = GetInMemoryDbContext(dbName);
            var seededMaterial = new Material
            {
                Name = "CLIPS ROJOS",
                Unit = "Metro",
                PricePurchase = 50,
                PhotoUrl = "old",
                IsActive = true
            };
            seedContext.Materials.Add(seededMaterial);
            await seedContext.SaveChangesAsync();
            var insertedId = seededMaterial.Id;
            await seedContext.DisposeAsync();


            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext())
                       .Returns(() => GetInMemoryDbContext(dbName));

            var service = new MaterialService(mockFactory.Object);

            //  se mantiene el mismo Id)
            var updatedMaterial = new Material
            {
                Id = insertedId,
                Name = "CLIPS VERDES",
                Unit = "Centímetro",
                PricePurchase = 40,
                PhotoUrl = "new",
                IsActive = true
            };
            var result = await service.UpdateMaterial(insertedId, updatedMaterial);

            Assert.NotNull(result);
            Assert.Equal(updatedMaterial.Name, result.Name);

            var verifyContext = GetInMemoryDbContext(dbName);
            var materialAfter = await verifyContext.Materials.FindAsync(insertedId);
            Assert.NotNull(materialAfter);
            Assert.Equal("CLIPS VERDES", materialAfter.Name);
            Assert.Equal("Centímetro", materialAfter.Unit);
            Assert.Equal(40, materialAfter.PricePurchase);
            Assert.Equal("new", materialAfter.PhotoUrl);

            await verifyContext.Database.EnsureDeletedAsync();
            await verifyContext.DisposeAsync();
        }

       

    }


}
