using Blazorise.Extensions;
using LAFABRICA.Data.DB;
using LAFABRICA.Services;
using LAFABRICA.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace LAFABRICA.Tests.Services
{
    public class UserServiceTests
    {
        private IDbContextFactory<AppDbContext> CreateFactory()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new PooledDbContextFactory<AppDbContext>(options);
        }

        private async Task<Rol> CreateTestRole(IDbContextFactory<AppDbContext> factory)
        {
            using var context = factory.CreateDbContext();

            var role = new Rol { Name = "ADMIN", IsActive = 1 };
            context.Rols.Add(role);
            await context.SaveChangesAsync();
            return role;
        }

        [Fact]
        public async Task CreateValidUser()
        {
            var factory = CreateFactory();
            var service = new UserService(factory);

            var role = await CreateTestRole(factory);

            var user = UserValid.CreateValidUser(role.Id);

            var result = await service.Create(user);

            Assert.NotNull(result);
            Assert.Equal(1,(int) result.IsActive!);
            Assert.Equal(role.Name, result.UserType);
            Assert.NotEqual("", result.Password);
        }

        [Fact]
        public async Task CreateUserWhenEmailExists()
        {
            var factory = CreateFactory();
            var service = new UserService(factory);
            var role = await CreateTestRole(factory);

            var u1 = UserValid.CreateValidUser(role.Id);
            var u2 = UserValid.CreateValidUser(role.Id, Identification: "706660666");

            await service.Create(u1);
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.Create(u2));
        }

        [Fact]
        public async Task CreateUserWhenIdentificationExists()
        {
            var factory = CreateFactory();
            var service = new UserService(factory);
            var role = await CreateTestRole(factory);

            var u1 = UserValid.CreateValidUser(role.Id);
            var u2 = UserValid.CreateValidUser(role.Id, email: "user2email@gmail.com");

            await service.Create(u1);
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.Create(u2));
        }

        [Fact]
        public async Task CreateUserWhenRoleNotExist()
        {
            var factory = CreateFactory();
            var service = new UserService(factory);

            var user = UserValid.CreateValidUser(999); // rol inexistente

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.Create(user));
        }
    }
}
