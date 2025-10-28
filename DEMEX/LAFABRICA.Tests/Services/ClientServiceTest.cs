using LAFABRICA.Data.DB;
using LAFABRICA.Services;
using LAFABRICA.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace LAFABRICA.Tests.Services
{
    public class ClientServiceTests
    {
        private IDbContextFactory<AppDbContext> CreateFactory()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new PooledDbContextFactory<AppDbContext>(options);
        }

        [Fact]
        public async Task CreateValidClient()
        {
            var factory = CreateFactory();
            var service = new ClientService(factory);

            var client = ClientValid.CreateValidClient();

            var result = await service.Create(client);

            using var context = factory.CreateDbContext();
            var dbClient = await context.Clients.FindAsync(result.Id);

            Assert.NotNull(dbClient);
            Assert.Equal(client.Email, dbClient.Email);
        }

        [Fact]
        public async Task CreateClientWithExistEmail()
        {
            var factory = CreateFactory();
            var service = new ClientService(factory);

            var client1 = ClientValid.CreateValidClient();
            await service.Create(client1);

            var client2 = ClientValid.CreateValidClient(email: client1.Email);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.Create(client2));
        }

        [Fact]
        public async Task DeleteLogicClient()
        {
            var factory = CreateFactory();
            var service = new ClientService(factory);

            var client = ClientValid.CreateValidClient();
            var saved = await service.Create(client);

            await service.Delete(saved.Id);

            using var context = factory.CreateDbContext();
            var deletedClient = await context.Clients.FindAsync(saved.Id);

            Assert.Equal<byte>(0, deletedClient!.IsActive);
        }

        [Fact]
        public async Task DeleteWithNotClientExist()
        {
            var factory = CreateFactory();
            var service = new ClientService(factory);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.Delete(999));
        }

        [Fact]
        public async Task GetAllClientActives()
        {
            var factory = CreateFactory();
            var service = new ClientService(factory);

            await service.Create(ClientValid.CreateValidClient());
            await service.Create(ClientValid.CreateInactiveClient());

            var result = await service.GetAllClient();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetByIdNotExistClient()
        {
            var factory = CreateFactory();
            var service = new ClientService(factory);

            var result = await service.GetById(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateModifyClientValid()
        {
            var factory = CreateFactory();
            var service = new ClientService(factory);

            var original = await service.Create(ClientValid.CreateValidClient());

            var updated = ClientValid.CreateValidClient(email: "newemail@test.com");
            updated.Id = original.Id;

            var result = await service.Update(original.Id, updated);

            Assert.Equal("newemail@test.com", result.Email);
        }

        [Fact]
        public async Task UpdateWhenEmailAlreadyUsed()
        {
            var factory = CreateFactory();
            var service = new ClientService(factory);

            var c1 = await service.Create(ClientValid.CreateValidClient());
            var c2 = await service
                    .Create(ClientValid
                    .CreateValidClient(email: "client2email@test.com",
                                        phone: "2121-2121",
                                        managerPhone: "6666-6666"));

            var invalidClientUpdate = ClientValid.CreateValidClient(email: c1.Email);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.Update(c2.Id, invalidClientUpdate));
        }
    }
}

