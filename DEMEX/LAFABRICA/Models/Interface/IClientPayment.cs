using LAFABRICA.Data.DB;
using System.Collections.Generic; // Added for IEnumerable
using System.Threading.Tasks; // Added for Task

namespace LAFABRICA.Models.Interface
{
    public interface IClientPaymentService
    {
        // Get all payments (consider if filtering by order is more common)
        Task<IEnumerable<ClientPayment>> GetAllPayments();

        // Get payments for a specific order
        Task<IEnumerable<ClientPayment>> GetPaymentsByOrderId(int orderId);

        // Get a single payment by its unique ID
        Task<ClientPayment?> GetPaymentById(int id);

        // Create a new payment record
        Task<ClientPayment> CreatePayment(ClientPayment payment);

        // Update an existing payment record
        Task<ClientPayment> UpdatePayment(int id, ClientPayment payment);

        // Delete a payment record (consider if this should be logical delete)
        Task<bool> DeletePayment(int id);
    }
}