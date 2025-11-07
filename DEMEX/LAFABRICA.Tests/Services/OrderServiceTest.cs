using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using LAFABRICA.Data.DB;
using LAFABRICA.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LAFABRICA.Service.Tests
{
    public class OrderServiceTests
    {
        private AppDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new AppDbContext(options);
        }

        private async Task SeedDependencies(AppDbContext context, int clientId, int productId)
        {
            if (await context.Clients.AnyAsync(c => c.Id == clientId) == false)
            {
                context.Clients.Add(new Client
                {
                    Id = clientId,
                    Name = $"Cliente Test {clientId}",
                    Email = $"test{clientId}@lafabrica.com",
                    PhoneNumber = "555-1234",
                    Location = "Ciudad Test",
                    SpecificLocation = "Oficina Central",
                    Manager = "Manager Test",
                    ManagerPhoneNumber = "555-5678",
                    IsActive = 1
                });
            }

            if (await context.Products.AnyAsync(p => p.Id == productId) == false)
            {
                context.Products.Add(new Product
                {
                    Id = productId,
                    Name = $"Producto Test {productId}",
                    Category = "Test",
                    Complexity = "Baja",
                    Description = "Desc",
                    PriceBase = 10m,
                    IsActive = 1
                });
            }

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task Create_ShouldSaveOrderWithProducts_WhenCalled()
        {
            string dbName = "Test_CreateOrder";
            var testContext = GetInMemoryDbContext(dbName);
            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);
            var service = new OrderService(mockFactory.Object);

            await SeedDependencies(testContext, clientId: 1, productId: 10);

            var newOrder = new Order
            {
                ClientId = 1,
                TotalAmount = 500.00m,
                State = "Pendiente",
                Priority = "Normal",
                CreationDate = DateOnly.FromDateTime(DateTime.Now),
                DaliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                IsActive = 1,
                ProductOrders = new List<ProductOrder>
                {
                    new ProductOrder { IdProduct = 10, Quantity = 2 }
                }
            };

            var createdOrder = await service.Create(newOrder);

            Assert.NotNull(createdOrder);
            Assert.True(createdOrder.Id > 0);

            await using var assertContext = GetInMemoryDbContext(dbName);

            var orderInDb = await assertContext.Orders
                                         .Include(o => o.ProductOrders)
                                         .FirstOrDefaultAsync(o => o.Id == createdOrder.Id);

            Assert.NotNull(orderInDb);
            Assert.Equal(1, orderInDb.ClientId);
            Assert.Single(orderInDb.ProductOrders);
            Assert.Equal(2, orderInDb.ProductOrders.First().Quantity);
        }

        [Fact]
        public async Task GetById_ShouldLoadClientAndProducts_WhenOrderExists()
        {
            string dbName = "Test_GetOrder_Load";
            var testContext = GetInMemoryDbContext(dbName);

            await SeedDependencies(testContext, clientId: 2, productId: 20);

            var orderToTest = new Order
            {
                Id = 101,
                ClientId = 2,
                TotalAmount = 100m,
                State = "Abierta",
                Priority = "Alta",
                IsActive = 1,
                CreationDate = DateOnly.FromDateTime(DateTime.Now),
                DaliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                ProductOrders = new List<ProductOrder> { new ProductOrder { IdProduct = 20, Quantity = 1 } }
            };
            testContext.Orders.Add(orderToTest);
            await testContext.SaveChangesAsync();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);
            var service = new OrderService(mockFactory.Object);

            var foundOrder = await service.GetById(101);

            Assert.NotNull(foundOrder);
            Assert.Equal(101, foundOrder.Id);
            Assert.NotNull(foundOrder.Client);
            Assert.Single(foundOrder.ProductOrders);
            Assert.NotNull(foundOrder.ProductOrders.First().IdProductNavigation);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenOrderIsInactive()
        {
            string dbName = "Test_GetOrder_Inactive";
            var testContext = GetInMemoryDbContext(dbName);

            var inactiveOrder = new Order
            {
                Id = 102,
                ClientId = 1,
                TotalAmount = 50m,
                State = "Cerrada",
                Priority = "Baja",
                IsActive = 0,
                CreationDate = DateOnly.FromDateTime(DateTime.Now),
                DaliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };
            testContext.Orders.Add(inactiveOrder);
            await testContext.SaveChangesAsync();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);
            var service = new OrderService(mockFactory.Object);

            var foundOrder = await service.GetById(102);

            Assert.Null(foundOrder);
        }

        [Fact]
        public async Task Delete_ShouldSoftDeleteOrder_WhenOrderExists()
        {
            string dbName = "Test_DeleteOrder";
            var testContext = GetInMemoryDbContext(dbName);

            var orderToTest = new Order
            {
                Id = 200,
                ClientId = 1,
                TotalAmount = 250m,
                State = "Abierta",
                Priority = "Normal",
                IsActive = 1,
                CreationDate = DateOnly.FromDateTime(DateTime.Now),
                DaliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10))
            };
            testContext.Orders.Add(orderToTest);
            await testContext.SaveChangesAsync();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);
            var service = new OrderService(mockFactory.Object);

            bool result = await service.Delete(200);

            Assert.True(result);

            await using var assertContext = GetInMemoryDbContext(dbName);

            var orderInDb = await assertContext.Orders
                                         .FirstOrDefaultAsync(o => o.Id == 200);

            Assert.NotNull(orderInDb);
            Assert.Equal(0, orderInDb.IsActive);
        }

        [Fact]
        public async Task Update_ShouldModifyOrderAndSyncProductOrders_WhenOrderExists()
        {
            string dbName = "Test_UpdateOrder";
            var testContext = GetInMemoryDbContext(dbName);

            await SeedDependencies(testContext, clientId: 3, productId: 30);
            await SeedDependencies(testContext, clientId: 3, productId: 31);

            var originalOrder = new Order
            {
                Id = 300,
                ClientId = 3,
                TotalAmount = 50.00m,
                State = "Abierta",
                Priority = "Baja",
                IsActive = 1,
                CreationDate = DateOnly.FromDateTime(DateTime.Now),
                DaliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                ProductOrders = new List<ProductOrder> { new ProductOrder { IdProduct = 30, Quantity = 1 } }
            };
            testContext.Orders.Add(originalOrder);
            await testContext.SaveChangesAsync();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);
            var service = new OrderService(mockFactory.Object);

            var updatedOrder = new Order
            {
                Id = 300,
                ClientId = 3,
                TotalAmount = 999.99m,
                State = "En Progreso",
                Priority = "Media",
                IsActive = 1,
                CreationDate = originalOrder.CreationDate,
                DaliveryDate = originalOrder.DaliveryDate,
                ProductOrders = new List<ProductOrder>
                {
                    new ProductOrder { IdProduct = 31, Quantity = 5 }
                }
            };

            await service.Update(updatedOrder.Id, updatedOrder);

            await using var assertContext = GetInMemoryDbContext(dbName);
            var orderInDb = await assertContext.Orders
                                         .Include(o => o.ProductOrders)
                                         .FirstOrDefaultAsync(o => o.Id == 300);

            Assert.NotNull(orderInDb);
            Assert.Equal(999.99m, orderInDb.TotalAmount);
            Assert.Equal("En Progreso", orderInDb.State);

            Assert.Single(orderInDb.ProductOrders);
            var productOrder = orderInDb.ProductOrders.First();
            Assert.Equal(31, productOrder.IdProduct);
            Assert.Equal(5, productOrder.Quantity);
        }

        [Fact]
        public async Task GetAllOrders_ShouldReturnOnlyActiveOrdersAndLoadClient()
        {
            string dbName = "Test_GetAllOrders";
            var testContext = GetInMemoryDbContext(dbName);

            await SeedDependencies(testContext, clientId: 4, productId: 40);

            var activeOrder1 = new Order { Id = 401, ClientId = 4, TotalAmount = 10m, State = "A", Priority = "B", IsActive = 1, CreationDate = DateOnly.FromDateTime(DateTime.Now), DaliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)) };
            var inactiveOrder = new Order { Id = 402, ClientId = 4, TotalAmount = 20m, State = "I", Priority = "B", IsActive = 0, CreationDate = DateOnly.FromDateTime(DateTime.Now), DaliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)) };
            var activeOrder2 = new Order { Id = 403, ClientId = 4, TotalAmount = 30m, State = "A", Priority = "B", IsActive = 1, CreationDate = DateOnly.FromDateTime(DateTime.Now), DaliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)) };

            testContext.Orders.AddRange(activeOrder1, inactiveOrder, activeOrder2);
            await testContext.SaveChangesAsync();

            var mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(testContext);
            var service = new OrderService(mockFactory.Object);

            var activeOrders = await service.GetAllOrders();

            Assert.NotNull(activeOrders);
            Assert.Equal(2, activeOrders.Count());
            Assert.All(activeOrders, o => Assert.Equal(1, o.IsActive));
            Assert.All(activeOrders, o => Assert.NotNull(o.Client));
        }
    }
}