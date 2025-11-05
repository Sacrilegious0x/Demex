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
                    .ThenInclude(po => po.IdProductNavigation) // Carga detalles del producto
                .FirstOrDefaultAsync(o => o.Id == id && o.IsActive == 1); // Añadido filtro IsActive por si acaso
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

            //  Actualizar propiedades simples de la orden
            context.Entry(existingOrder).CurrentValues.SetValues(order);

            

            // Lista de productos que vienen del formulario/UI
            var updatedProductOrders = order.ProductOrders ?? new List<ProductOrder>();

            // Lista de productos actualmente en la base de datos para esta orden
            var dbProductOrders = existingOrder.ProductOrders.ToList();

            //  Identificar y ELIMINAR productos que ya no están en la lista actualizada
            var productsToRemove = dbProductOrders
                .Where(dbPo => !updatedProductOrders.Any(updPo => updPo.IdProduct == dbPo.IdProduct))
                .ToList();

            foreach (var productToRemove in productsToRemove)
            {
                // Removemos del contexto para que EF genere el DELETE
                context.ProductOrders.Remove(productToRemove);
            }

            //Identificar y AÑADIR o ACTUALIZAR productos
            foreach (var updatedPo in updatedProductOrders)
            {
                var existingPo = dbProductOrders.FirstOrDefault(dbPo => dbPo.IdProduct == updatedPo.IdProduct);

                if (existingPo != null)
                {
                    //  El producto ya existe, solo actualiza la cantidad
                    existingPo.Quantity = updatedPo.Quantity;
                    // No es necesario reasignar IdProductNavigation aquí si ya estaba cargado
                }
                else
                {
                    //  El producto es nuevo en la orden
                    //    Creamos la nueva entidad y la añadimos al contexto.
                    //    Es importante asignar la navegación a la orden existente.
                    var newPo = new ProductOrder
                    {
                        IdOrder = existingOrder.Id, // Asigna el ID de la orden existente
                        IdProduct = updatedPo.IdProduct,
                        Quantity = updatedPo.Quantity,
                         
                    };
                    // Añadimos la nueva relación a la lista rastreada por EF
                    existingOrder.ProductOrders.Add(newPo);
                    // O directamente al contexto si prefieres: _context.ProductOrders.Add(newPo);
                }
            }
            


            
            // borrar los registros viejos en PRODUCT_ORDER y añadir los nuevos.
            await context.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByClientId(int clientId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Orders
                .Where(o => o.ClientId == clientId && o.IsActive == 1)
                .Include(o => o.ProductOrders)
                    .ThenInclude(po => po.IdProductNavigation)
                .Include(o => o.Client)
                .ToListAsync();
        }

        public async Task<bool> HasPendingPaymentAsync(int clientId)
        {
            using var context = _contextFactory.CreateDbContext();

            var order = await context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.ClientId == clientId && o.IsActive == 1);

            if (order == null)
                throw new InvalidOperationException("La orden no existe o está inactiva.");

          
            return order.Advancement < order.TotalAmount;
        }


    }
}
