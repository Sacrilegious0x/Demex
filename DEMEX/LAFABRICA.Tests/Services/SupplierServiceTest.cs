using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using LAFABRICA.Data.DB;
using LAFABRICA.Services;
using LAFABRICA.Models.AuxiliarDTOS;
using LAFABRICA.Models; 
using System.Collections.Generic;

namespace LAFABRICA.Tests.Services
{
    public class SupplierServiceTest
    {
       
        private AppDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddSupplierAsync_ShouldAddSupplier_WhenValid()
        {
            
            string dbName = Guid.NewGuid().ToString();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext())
                       .Returns(() => GetInMemoryDbContext(dbName));

            var service = new SupplierService(mockFactory.Object);

            var newSupplierDto = new SupplierDto
            {
                Name = "Proveedor Nuevo",
                Address = "Ruta 32 400 mts al sur",
                Phone = "8888-8888",
                Email = "provnuevo@example.com",
                DateLastPurchase = null,
                Notes = "Nota de prueba"
            };

           
            var result = await service.AddSupplierAsync(newSupplierDto);

            
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal(newSupplierDto.Name, result.Name);
            Assert.Equal(newSupplierDto.Email, result.Email);

            // VERIFY: leer desde otra instancia del contexto para confirmar persistencia
            using var verifyContext = GetInMemoryDbContext(dbName);
            var saved = await verifyContext.Suppliers.FindAsync(result.Id);
            Assert.NotNull(saved);
            Assert.Equal(newSupplierDto.Email, saved.Email);
            Assert.Equal(newSupplierDto.Phone, saved.Phone);
            Assert.True(saved.IsActive);


            await verifyContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task AddSupplierAsync_ShouldThrowInvalidOperationException_WhenPhoneExists()
        {
            // ARRANGE
            string dbName = Guid.NewGuid().ToString();

            // Seed con un proveedor activo que ya tiene el teléfono
            using (var seedContext = GetInMemoryDbContext(dbName))
            {
                seedContext.Suppliers.Add(new Supplier
                {
                    Name = "ExistenteTel",
                    Address = "Addr",
                    Phone = "5555-1234",
                    Email = "unique-email@example.com",
                    DateLastPurchase = null,
                    Notes = "",
                    IsActive = true
                });
                await seedContext.SaveChangesAsync();
            }

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext())
                       .Returns(() => GetInMemoryDbContext(dbName));

            var service = new SupplierService(mockFactory.Object);

            var newSupplierDto = new SupplierDto
            {
                Name = "NuevoConTelDuplicado",
                Address = "Addr 2",
                Phone = "5555-1234", // mismo teléfono 
                Email = "another-email@example.com",
                DateLastPurchase = null,
                Notes = ""
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await service.AddSupplierAsync(newSupplierDto);
            });

            // limpieza opcional
            using var cleanup = GetInMemoryDbContext(dbName);
            await cleanup.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task GetSupplierByIdAsync_ShouldReturnSupplier_WhenExists()
        {
            string dbName = Guid.NewGuid().ToString();

            var seedContext = GetInMemoryDbContext(dbName);
            var seededSupplier = new Supplier
            {
                Name = "Proveedor Ejemplo",
                Address = "Calle 1",
                Phone = "8000-1111",
                Email = "proveedor.ejemplo@example.com",
                DateLastPurchase = null,
                Notes = "Notas",
                IsActive = true
            };
            seedContext.Suppliers.Add(seededSupplier);
            await seedContext.SaveChangesAsync();
            var insertedId = seededSupplier.Id;
            await seedContext.DisposeAsync();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext())
                       .Returns(() => GetInMemoryDbContext(dbName));

            var service = new SupplierService(mockFactory.Object);

            var result = await service.GetSupplierByIdAsync(insertedId);

            Assert.NotNull(result);
            Assert.Equal(insertedId, result.Id);
            Assert.Equal("Proveedor Ejemplo", result.Name);
            Assert.Equal("proveedor.ejemplo@example.com", result.Email);
            Assert.Equal("8000-1111", result.Phone);
            Assert.True(result.IsActive);

            using var cleanup = GetInMemoryDbContext(dbName);
            await cleanup.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task DeleteSupplierAsync_ShouldReturnTrueAndSetIsActiveFalse_WhenSupplierExists()
        {
            string dbName = Guid.NewGuid().ToString();

            var seedContext = GetInMemoryDbContext(dbName);
            var seededSupplier = new Supplier
            {
                Name = "Proveedor A Borrar",
                Address = "Calle X",
                Phone = "7000-2222",
                Email = "borrar@example.com",
                DateLastPurchase = null,
                Notes = "",
                IsActive = true
            };
            seedContext.Suppliers.Add(seededSupplier);
            await seedContext.SaveChangesAsync();
            var insertedId = seededSupplier.Id;
            await seedContext.DisposeAsync();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext())
                       .Returns(() => GetInMemoryDbContext(dbName));

            var service = new SupplierService(mockFactory.Object);

            var result = await service.DeleteSupplierAsync(insertedId);

            Assert.True(result);

            var verifyContext = GetInMemoryDbContext(dbName);
            var supplierAfter = await verifyContext.Suppliers.FindAsync(insertedId);
            Assert.NotNull(supplierAfter);
            Assert.False(supplierAfter.IsActive);

            await verifyContext.Database.EnsureDeletedAsync();
            await verifyContext.DisposeAsync();
        }

        [Fact]
        public async Task UpdateSupplierAsync_ShouldReturnTrueAndUpdate_WhenValid()
        {
            string dbName = Guid.NewGuid().ToString();

            var seedContext = GetInMemoryDbContext(dbName);

            var seedObject = new Supplier
            {
                Name = "Original",
                Address = "C/ A",
                Phone = "6000-0000",
                Email = "orig@example.com",
                IsActive = true
            };

            seedContext.Suppliers.Add(seedObject);
                
            await seedContext.SaveChangesAsync();

            var insertedId = seedObject.Id;

            await seedContext.DisposeAsync();
            

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(() => GetInMemoryDbContext(dbName));
            var service = new SupplierService(mockFactory.Object);

           
                
                var updatedDto = new SupplierDto
                {
                    Id = insertedId,
                    Name = "Original Modificado",
                    Address = "C/ B",
                    Phone = "6000-1111",
                    Email = "orig-mod@example.com",
                    DateLastPurchase = null,
                    Notes = "nota",
                    IsActive = true
                };

                var result = await service.UpdateSupplierAsync(updatedDto);


                Assert.True(result);
                var verifyContext = GetInMemoryDbContext(dbName);
                var testSupplier = await verifyContext.Suppliers.FindAsync(insertedId);
                Assert.NotNull(testSupplier);
                Assert.Equal("Original Modificado", testSupplier.Name);
                Assert.Equal("6000-1111", testSupplier.Phone);
                Assert.Equal("orig-mod@example.com", testSupplier.Email);
            

            using var cleanup = GetInMemoryDbContext(dbName);
            await cleanup.Database.EnsureDeletedAsync();
        }
    }

}