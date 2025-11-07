using LAFABRICA.Data.DB;
using LAFABRICA.Services;
using LAFABRICA.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace LAFABRICA.Tests.Services
{
    public class EmployeePaymentServiceTests
    {
        private IDbContextFactory<AppDbContext> CreateFactory()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new PooledDbContextFactory<AppDbContext>(options);
        }

        [Fact]
        public async Task CreatePayment()
        {
            var factory = CreateFactory();
            var service = new EmployeePaymentService(factory);

            var payment = EmployeePaymentValid.CreateValidPayment();

            var result = await service.CreatePaymentAsync(payment);

            using var context = factory.CreateDbContext();
            var saved = await context.EmployeePayments.FindAsync(result.Id);

            Assert.NotNull(saved);
            Assert.Equal(payment.Description, saved.Description);
            Assert.Equal(payment.State, saved.State);
            Assert.Equal(payment.TotalAmount, saved.TotalAmount);
        }

        [Fact]
        public async Task GetAllPaymentsActive()
        {
            var factory = CreateFactory();
            using (var context = factory.CreateDbContext())
            {
                // Inserta un rol porque User lo necesita
                var role = new Rol { Id = 2, Name = "EMPLEADO", IsActive = 1 };
                context.Rols.Add(role);

                // Inserta usuario válido (empleado)
                var user = UserValid.CreateValidUser(role.Id);
                context.Users.Add(user);

                // Inserta pagos con relación al usuario
                context.EmployeePayments.Add(EmployeePaymentValid.CreateValidPayment(employeeId: user.Id));
                context.EmployeePayments.Add(EmployeePaymentValid.CreateInactivePayment(employeeId: user.Id));

                await context.SaveChangesAsync();
            }

            var service = new EmployeePaymentService(factory);
            var result = await service.GetAllPaymentsAsync();

            Assert.Single(result);
            Assert.All(result, p => Assert.True(p.IsActive));
        }

        [Fact]
        public async Task UpdatePayment()
        {
            var factory = CreateFactory();
            using (var context = factory.CreateDbContext())
            {
                context.EmployeePayments.Add(EmployeePaymentValid.CreateValidPayment());
                await context.SaveChangesAsync();
            }

            var service = new EmployeePaymentService(factory);
            using var setupContext = factory.CreateDbContext();
            var existing = await setupContext.EmployeePayments.FirstAsync();

            var updated = new EmployeePayment
            {
                Id = existing.Id,
                Description = "Pago actualizado",
                State = "Pagado",
                TotalAmount = 20000
            };

            var success = await service.UpdatePaymentAsync(updated);

            Assert.True(success);

            using var verify = factory.CreateDbContext();
            var modified = await verify.EmployeePayments.FindAsync(existing.Id);

            Assert.Equal("Pago actualizado", modified!.Description);
            Assert.Equal("Pagado", modified.State);
            Assert.Equal(20000, modified.TotalAmount);
        }

        [Fact]
        public async Task LogicDeletePayment()
        {
            var factory = CreateFactory();
            using (var context = factory.CreateDbContext())
            {
                context.EmployeePayments.Add(EmployeePaymentValid.CreateValidPayment());
                await context.SaveChangesAsync();
            }

            var service = new EmployeePaymentService(factory);
            using var setupContext = factory.CreateDbContext();
            var existing = await setupContext.EmployeePayments.FirstAsync();

            var result = await service.DeletePaymentAsync(existing.Id);

            Assert.True(result);

            using var verify = factory.CreateDbContext();
            var deleted = await verify.EmployeePayments.FindAsync(existing.Id);
            Assert.False(deleted!.IsActive);
        }

        [Fact]
        public async Task HasPendingPaymentExists()
        {
            var factory = CreateFactory();
            using (var context = factory.CreateDbContext())
            {
                context.EmployeePayments.Add(EmployeePaymentValid.CreatePendingPayment(employeeId: 7));
                await context.SaveChangesAsync();
            }

            var service = new EmployeePaymentService(factory);
            var result = await service.HasPendingPaymentAsync(7);

            Assert.True(result);
        }

        [Fact]
        public async Task RestorePayment_ShouldReactivatePayment()
        {
            var factory = CreateFactory();
            using (var context = factory.CreateDbContext())
            {
                context.EmployeePayments.Add(EmployeePaymentValid.CreateInactivePayment());
                await context.SaveChangesAsync();
            }

            var service = new EmployeePaymentService(factory);
            using var setupContext = factory.CreateDbContext();
            var inactive = await setupContext.EmployeePayments.FirstAsync();

            var success = await service.RestorePaymentAsync(inactive.Id);

            Assert.True(success);

            using var verify = factory.CreateDbContext();
            var restored = await verify.EmployeePayments.FindAsync(inactive.Id);
            Assert.True(restored!.IsActive);
        }

        [Fact]
        public async Task GetPaymentsByEmployeeId()
        {
            var factory = CreateFactory();
            using (var context = factory.CreateDbContext())
            {
                context.EmployeePayments.Add(EmployeePaymentValid.CreateValidPayment(employeeId: 1));
                context.EmployeePayments.Add(EmployeePaymentValid.CreatePaidPayment(employeeId: 1));
                context.EmployeePayments.Add(EmployeePaymentValid.CreateValidPayment(employeeId: 2));
                await context.SaveChangesAsync();
            }

            var service = new EmployeePaymentService(factory);
            var results = await service.GetPaymentsByEmployeeIdAsync(1);

            Assert.Equal(2, results.Count());
            Assert.All(results, p => Assert.Equal(1, p.EmployeeId));
        }

        [Fact]
        public async Task GetPaymentByIdIfNotExist()
        {
            var factory = CreateFactory();
            var service = new EmployeePaymentService(factory);

            var result = await service.GetPaymentByIdAsync(999);

            Assert.Null(result);
        }
    }
}
