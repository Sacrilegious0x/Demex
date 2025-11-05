using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;

namespace LAFABRICA.Services
{
    public class EmployeePaymentService : IEmployeePaymentService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public EmployeePaymentService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<EmployeePayment> CreatePaymentAsync(EmployeePayment payment)
        {
            using var context = _contextFactory.CreateDbContext();
            context.EmployeePayments.Add(payment);
            await context.SaveChangesAsync();
            return payment;
        }

        public async Task<IEnumerable<EmployeePayment>> GetAllPaymentsAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.EmployeePayments
                .Include(p => p.Employee)
                .Include(p => p.PayEmployeeProducts)
                .ToListAsync();
        }

        public async Task<EmployeePayment?> GetPaymentByIdAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.EmployeePayments
                .Include(p => p.Employee)
                .Include(p => p.PayEmployeeProducts)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<EmployeePayment>> GetPaymentsByEmployeeIdAsync(int employeeId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.EmployeePayments
                .Where(p => p.EmployeeId == employeeId)
                .Include(p => p.PayEmployeeProducts)
                .ToListAsync();
        }

        public async Task<bool> UpdatePaymentAsync(EmployeePayment payment)
        {
            using var context = _contextFactory.CreateDbContext();
            var existing = await context.EmployeePayments.FindAsync(payment.Id);
            if (existing == null) return false;

            existing.Description = payment.Description;
            existing.State = payment.State;
            existing.TotalAmount = payment.TotalAmount;
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var payment = await context.EmployeePayments.FindAsync(id);
            if (payment == null) return false;

            context.EmployeePayments.Remove(payment);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasPendingPaymentAsync(int employeeId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.EmployeePayments
                .AnyAsync(p => p.EmployeeId == employeeId && p.State == "Pendiente");
        }


    }
}
