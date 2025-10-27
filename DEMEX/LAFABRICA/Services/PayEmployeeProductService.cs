using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;

namespace LAFABRICA.Services
{
    public class PayEmployeeProductService : IPayEmployeeProductService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public PayEmployeeProductService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<PayEmployeeProduct> AddProductToPaymentAsync(PayEmployeeProduct product)
        {
            using var context = _contextFactory.CreateDbContext();
            context.PayEmployeeProducts.Add(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<PayEmployeeProduct>> GetAllPayEmployeeProductsAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.PayEmployeeProducts
                .Include(p => p.Product)
                .Include(p => p.EmployeePayment)
                .ToListAsync();
        }

        public async Task<IEnumerable<PayEmployeeProduct>> GetByEmployeePaymentIdAsync(int employeePaymentId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.PayEmployeeProducts
                .Include(p => p.Product)
                .Where(p => p.EmployeePaymentId == employeePaymentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PayEmployeeProduct>> GetByProductIdAsync(int productId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.PayEmployeeProducts
                .Include(p => p.EmployeePayment)
                .Where(p => p.ProductId == productId)
                .ToListAsync();
        }

        public async Task<bool> UpdatePayEmployeeProductAsync(PayEmployeeProduct product)
        {
            using var context = _contextFactory.CreateDbContext();
            var existing = await context.PayEmployeeProducts.FindAsync(product.Id);
            if (existing == null) return false;

            existing.Quantity = product.Quantity;
            existing.UnitPrice = product.UnitPrice;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePayEmployeeProductAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var product = await context.PayEmployeeProducts.FindAsync(id);
            if (product == null) return false;

            context.PayEmployeeProducts.Remove(product);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
