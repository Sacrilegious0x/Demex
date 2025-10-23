using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;

namespace LAFABRICA.Services
{
    public class OrderService : IOrderService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public OrderService(IDbContextFactory<AppDbContext> context)
        {
            _contextFactory = context;
        }

        public async Task<Order> Create(Order order)
        {
            // Añadimos la orden completa. EF Core se encargará de crear el registro principal
            // y luego las relaciones en la tabla ProductOrder.
            using var context = _contextFactory.CreateDbContext();
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> Delete(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var order = await context.Orders.FindAsync(id);
            if (order == null)
                return false;

            order.IsActive = 0; // Borrado lógico
            context.Orders.Update(order);
            await   context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            // Añadido filtro para IsActive y cargamos datos del cliente para la tabla.
            using var context = _contextFactory.CreateDbContext();
            return await context.Orders
                                 .Where(o => o.IsActive == 1)
                                 .Include(o => o.Client)
                                 .ToListAsync();
        }

     public async Task<Order?> GetById(int id)
        {
            // Usamos FirstOrDefaultAsync con Include para traer los productos de la orden
            using var context = _contextFactory.CreateDbContext();
            return await context.Orders
                .Include(o => o.Client)
                .Include(o => o.ProductOrders)
                    // === ESTA ES LA LÍNEA CORREGIDA ===
                    .ThenInclude(po => po.IdProductNavigation) 
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> Update(int id, Order order)
        {
            using var context = _contextFactory.CreateDbContext();
            var existingOrder = await context.Orders
                .Include(o => o.ProductOrders) // Cargar las relaciones existentes
                .FirstOrDefaultAsync(o => o.Id == id);

            if (existingOrder == null)
            {
                throw new KeyNotFoundException($"La orden con el id {id} no fue encontrada.");
            }

            // 1. Actualizar propiedades simples de la orden
            context.Entry(existingOrder).CurrentValues.SetValues(order);

            // 2. Sincronizar los productos de la orden
            // Primero, borramos la lista de relaciones que tenía el objeto en memoria
            existingOrder.ProductOrders.Clear();

            // Segundo, añadimos las nuevas relaciones que vienen del formulario
            if (order.ProductOrders != null && order.ProductOrders.Any())
            {
                foreach (var productOrder in order.ProductOrders)
                {
                    existingOrder.ProductOrders.Add(new ProductOrder
                    {
                        IdProduct = productOrder.IdProduct,
                        Quantity = productOrder.Quantity
                    });
                }
            }

            // 3. Guardar todo. EF Core se encargará de comparar las listas,
            // borrar los registros viejos en PRODUCT_ORDER y añadir los nuevos.
            await context.SaveChangesAsync();
            return order;
        }
    }
}
