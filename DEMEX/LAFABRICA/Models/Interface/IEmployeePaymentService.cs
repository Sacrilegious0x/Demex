using LAFABRICA.Data.DB;

namespace LAFABRICA.Models.Interface
{
    public interface IEmployeePaymentService
    {
        // Crear un nuevo pago
        Task<EmployeePayment> CreatePaymentAsync(EmployeePayment payment);

        // Obtener todos los pagos
        Task<IEnumerable<EmployeePayment>> GetAllPaymentsAsync();

        // Obtener un pago por ID
        Task<EmployeePayment?> GetPaymentByIdAsync(int id);

        // Obtener pagos por empleado
        Task<IEnumerable<EmployeePayment>> GetPaymentsByEmployeeIdAsync(int employeeId);

        // Actualizar datos del pago (por ejemplo estado o descripción)
        Task<bool> UpdatePaymentAsync(EmployeePayment payment);

        // Eliminar un pago (opcional)
        Task<bool> DeletePaymentAsync(int id);

   
    }
}
