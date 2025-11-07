using LAFABRICA.Data.DB;

namespace LAFABRICA.Models.Interface
{
    public interface IPayEmployeeProductService
    {
        // Agregar un producto pagado a un pago
        Task<PayEmployeeProduct> AddProductToPaymentAsync(PayEmployeeProduct product);

        // Obtener todos los productos pagados (para auditoría o reportes)
        Task<IEnumerable<PayEmployeeProduct>> GetAllPayEmployeeProductsAsync();

        // Obtener productos pagados asociados a un pago específico
        Task<IEnumerable<PayEmployeeProduct>> GetByEmployeePaymentIdAsync(int employeePaymentId);

        // Obtener productos pagados por producto específico (opcional)
        Task<IEnumerable<PayEmployeeProduct>> GetByProductIdAsync(int productId);

        // Actualizar la cantidad o el precio unitario
        Task<bool> UpdatePayEmployeeProductAsync(PayEmployeeProduct product);

        // Eliminar un producto del pago
        Task<bool> DeletePayEmployeeProductAsync(int id);
    }
}
