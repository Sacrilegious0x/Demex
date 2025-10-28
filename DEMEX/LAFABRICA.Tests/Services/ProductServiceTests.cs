using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using LAFABRICA.Data.DB;
using LAFABRICA.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LAFABRICA.Service.Tests
{
    public class ProductServiceTests
    {
        private AppDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task Create_ShouldSaveProductWithMaterials_WhenCalled()
        {
            string dbName = "Test_CreateProduct";
            var testContext = GetInMemoryDbContext(dbName);
            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);

            var service = new ProductService(mockFactory.Object);

            var newProduct = new Product
            {
                Name = "Producto de Prueba",
                PriceBase = 100m,
                Category = "Hogar y Decoración",
                Description = "Descripción de prueba",
                Complexity = "Baja",
                IsActive = 1,
                ProductMaterials = new List<ProductMaterial>
                {
                    new ProductMaterial { MaterialId = 10 },
                    new ProductMaterial { MaterialId = 20 }
                }
            };

            var createdProduct = await service.Create(newProduct);

            Assert.NotNull(createdProduct);

            await using var assertContext = GetInMemoryDbContext(dbName);

            var productInDb = await assertContext.Products
                                         .Include(p => p.ProductMaterials)
                                         .FirstOrDefaultAsync(p => p.Id == createdProduct.Id);

            Assert.NotNull(productInDb);
            Assert.Equal(2, productInDb.ProductMaterials.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnActiveProduct_WhenProductExists()
        {
            string dbName = "Test_GetProduct_Active";
            var testContext = GetInMemoryDbContext(dbName);

            var activeProduct = new Product
            {
                Id = 101,
                Name = "Producto Activo",
                IsActive = 1,
                Category = "Categoría Prueba",
                Description = "Descripción de prueba",
                Complexity = "Media"
            };
            testContext.Products.Add(activeProduct);
            await testContext.SaveChangesAsync();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);
            var service = new ProductService(mockFactory.Object);

            var foundProduct = await service.GetById(101);

            Assert.NotNull(foundProduct);
            Assert.Equal(101, foundProduct.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenProductIsInactive()
        {
            string dbName = "Test_GetProduct_Inactive";
            var testContext = GetInMemoryDbContext(dbName);

            var inactiveProduct = new Product
            {
                Id = 102,
                Name = "Producto Inactivo",
                IsActive = 0,
                Category = "Categoría Prueba",
                Description = "Descripción de prueba",
                Complexity = "Alta"
            };
            testContext.Products.Add(inactiveProduct);
            await testContext.SaveChangesAsync();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);
            var service = new ProductService(mockFactory.Object);

            var foundProduct = await service.GetById(102);

            Assert.Null(foundProduct);
        }

        [Fact]
        public async Task Delete_ShouldSoftDeleteProduct_WhenProductExists()
        {
            string dbName = "Test_DeleteProduct";
            var testContext = GetInMemoryDbContext(dbName);

            var productToTest = new Product
            {
                Id = 200,
                Name = "Producto a Borrar",
                IsActive = 1,
                Category = "Categoría Prueba",
                Description = "Descripción de prueba",
                Complexity = "Baja"
            };
            testContext.Products.Add(productToTest);
            await testContext.SaveChangesAsync();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);
            var service = new ProductService(mockFactory.Object);

            bool result = await service.Delete(200);

            Assert.True(result);

            await using var assertContext = GetInMemoryDbContext(dbName);

            var productInDb = await assertContext.Products
                                         .FirstOrDefaultAsync(p => p.Id == 200);

            Assert.NotNull(productInDb);
            Assert.Equal(0, productInDb.IsActive);
        }

        [Fact]
        public async Task Update_ShouldModifyProductAndMaterials_WhenProductExists()
        {
            string dbName = "Test_UpdateProduct";
            var testContext = GetInMemoryDbContext(dbName);

            var originalProduct = new Product
            {
                Id = 300,
                Name = "Producto Original",
                PriceBase = 50.00m,
                Category = "Vieja",
                Description = "Descripción Antigua",
                Complexity = "Baja",
                IsActive = 1,
                ProductMaterials = new List<ProductMaterial> { new ProductMaterial { MaterialId = 50 } }
            };
            testContext.Products.Add(originalProduct);
            await testContext.SaveChangesAsync();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);
            var service = new ProductService(mockFactory.Object);

            var updatedProduct = new Product
            {
                Id = 300,
                Name = "Producto Actualizado",
                PriceBase = 75.00m,
                Category = "Nueva",
                Description = "Descripción Novedosa",
                Complexity = "Alta",
                IsActive = 1,
                ProductMaterials = new List<ProductMaterial>
                {
                    new ProductMaterial { MaterialId = 60 },
                    new ProductMaterial { MaterialId = 70 }
                }
            };

            await service.Update(updatedProduct.Id, updatedProduct);

            await using var assertContext = GetInMemoryDbContext(dbName);
            var productInDb = await assertContext.Products
                                         .Include(p => p.ProductMaterials)
                                         .FirstOrDefaultAsync(p => p.Id == 300);

            Assert.NotNull(productInDb);
            Assert.Equal("Producto Actualizado", productInDb.Name);
            Assert.Equal("Nueva", productInDb.Category);

            Assert.Equal(2, productInDb.ProductMaterials.Count);
            var materialIds = productInDb.ProductMaterials.Select(pm => pm.MaterialId).ToList();
            Assert.Contains(60, materialIds);
            Assert.Contains(70, materialIds);
            Assert.DoesNotContain(50, materialIds);
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnOnlyActiveProducts()
        {
            string dbName = "Test_GetAllProducts";
            var testContext = GetInMemoryDbContext(dbName);

            testContext.Products.Add(new Product { Id = 401, Name = "A1", IsActive = 1, Category = "C", Description = "D", Complexity = "B" });
            testContext.Products.Add(new Product { Id = 402, Name = "I1", IsActive = 0, Category = "C", Description = "D", Complexity = "B" });
            testContext.Products.Add(new Product { Id = 403, Name = "A2", IsActive = 1, Category = "C", Description = "D", Complexity = "B" });
            await testContext.SaveChangesAsync();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);
            var service = new ProductService(mockFactory.Object);

            var activeProducts = await service.GetAllProducts();

            Assert.NotNull(activeProducts);
            Assert.Equal(2, activeProducts.Count());
            Assert.All(activeProducts, p => Assert.Equal(1, p.IsActive));
        }
    }
}