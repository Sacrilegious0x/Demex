using LAFABRICA.Data.DB;
using System.Collections.Generic; // Added for IEnumerable
using System.Threading.Tasks; // Added for Task

namespace LAFABRICA.Models.Interface
{
    public interface IClientPaymentService
    {
        
        Task<IEnumerable<ClientPayment>> GetAllPayments();

        
        Task<IEnumerable<ClientPayment>> GetPaymentsByOrderId(int orderId);

        Task<ClientPayment?> GetPaymentById(int id);

        
        Task<ClientPayment> CreatePayment(ClientPayment payment);

       
        Task<ClientPayment> UpdatePayment(int id, ClientPayment payment);

        
        Task<bool> DeletePayment(int id);
    }
}