using LAFABRICA.Data.DB;

namespace LAFABRICA.Models.Interface
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrders();
        Task<Order?> GetById(int id);
        Task<Order> Create(Order order);
        Task<Order> Update(int id, Order order);
        Task<bool> Delete(int id);
         Task<IEnumerable<Order>> GetOrdersByClientId(int clientId);
        
        }
}