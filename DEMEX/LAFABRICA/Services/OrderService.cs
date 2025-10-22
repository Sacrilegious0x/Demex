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
            // Esta lógica es correcta, EF maneja las relaciones si IdOrderNavigation está asignado
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            order.IsActive = 0; // Borrado lógico
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _context.Orders
                                .Where(o => o.IsActive == 1)
                                .Include(o => o.Client)
                                .ToListAsync();
        }

        public async Task<Order?> GetById(int id)
        {
            // Esta lógica es correcta, carga todo lo necesario
            return await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.ProductOrders)
                    .ThenInclude(po => po.IdProductNavigation) // Carga detalles del producto
                .FirstOrDefaultAsync(o => o.Id == id && o.IsActive == 1); // Añadido filtro IsActive por si acaso
        }

        // === MÉTODO UPDATE CORREGIDO ===
        public async Task<Order> Update(int id, Order order)
        {
            var existingOrder = await _context.Orders
                .Include(o => o.ProductOrders) // Cargar las relaciones existentes es crucial
                .FirstOrDefaultAsync(o => o.Id == id);

            if (existingOrder == null)
            {
                throw new KeyNotFoundException($"La orden con el id {id} no fue encontrada.");
            }

            // 1. Actualizar propiedades simples de la orden (como ClienteId, Estado, Total, etc.)
            //    SetValues es eficiente para esto.
            _context.Entry(existingOrder).CurrentValues.SetValues(order);

            // === INICIO DE LA CORRECCIÓN: Sincronizar ProductOrders ===

            // Lista de productos que vienen del formulario/UI
            var updatedProductOrders = order.ProductOrders ?? new List<ProductOrder>();

            // Lista de productos actualmente en la base de datos para esta orden
            var dbProductOrders = existingOrder.ProductOrders.ToList();

            // 1. Identificar y ELIMINAR productos que ya no están en la lista actualizada
            var productsToRemove = dbProductOrders
                .Where(dbPo => !updatedProductOrders.Any(updPo => updPo.IdProduct == dbPo.IdProduct))
                .ToList();

            foreach (var productToRemove in productsToRemove)
            {
                // Removemos del contexto para que EF genere el DELETE
                _context.ProductOrders.Remove(productToRemove);
            }

            // 2. Identificar y AÑADIR o ACTUALIZAR productos
            foreach (var updatedPo in updatedProductOrders)
            {
                var existingPo = dbProductOrders.FirstOrDefault(dbPo => dbPo.IdProduct == updatedPo.IdProduct);

                if (existingPo != null)
                {
                    // 3. ACTUALIZAR: El producto ya existe, solo actualiza la cantidad
                    existingPo.Quantity = updatedPo.Quantity;
                    // No es necesario reasignar IdProductNavigation aquí si ya estaba cargado
                }
                else
                {
                    // 4. AÑADIR: El producto es nuevo en la orden
                    //    Creamos la nueva entidad y la añadimos al contexto.
                    //    Es importante asignar la navegación a la orden existente.
                    var newPo = new ProductOrder
                    {
                        IdOrder = existingOrder.Id, // Asigna el ID de la orden existente
                        IdProduct = updatedPo.IdProduct,
                        Quantity = updatedPo.Quantity,
                        // NO asignamos IdOrderNavigation ni IdProductNavigation aquí,
                        // EF lo manejará basado en los IDs si las claves foráneas están bien configuradas.
                        // Opcionalmente, podrías buscar el producto y asignarlo si quieres ser explícito:
                        // IdProductNavigation = await _context.Products.FindAsync(updatedPo.IdProduct) 
                    };
                    // Añadimos la nueva relación a la lista rastreada por EF
                    existingOrder.ProductOrders.Add(newPo);
                    // O directamente al contexto si prefieres: _context.ProductOrders.Add(newPo);
                }
            }
            // === FIN DE LA CORRECCIÓN ===


            // 5. Guardar todos los cambios (Updates de Order, Deletes e Inserts/Updates de ProductOrder)
            await _context.SaveChangesAsync();

            // Devolvemos la entidad actualizada (aunque 'order' podría no tener todas las navegaciones)
            // Es mejor devolver 'existingOrder' si se necesita la entidad completa post-guardado.
            return existingOrder;
        }
    }
}
