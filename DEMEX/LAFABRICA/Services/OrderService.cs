using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;

namespace LAFABRICA.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> Create(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

      
        public async Task<bool> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

          
            order.IsActive = 0;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<Order>> GetAllOrders()
        {
          
            return await _context.Orders
                                 .Include(o => o.Client)
                                 .Include(o => o.ProductOrders) 
                                 .ToListAsync();
        }

        public async Task<Order?> GetById(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<Order> Update(int id, Order order)
        {
            var oldOrder = await _context.Orders.FindAsync(id);
            if (oldOrder == null)
                throw new KeyNotFoundException($"La orden con el id {id} no fue encontrada.");

            _context.Entry(oldOrder).CurrentValues.SetValues(order);
            await _context.SaveChangesAsync();
            return order;
        }
    }
}