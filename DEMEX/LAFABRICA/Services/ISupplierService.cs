using LAFABRICA.Models;

namespace LAFABRICA.Services
{
    public interface ISupplierService
    {

        Task<List<SupplierDto>> GetSuppliersAsync();
        Task<SupplierDto> AddSupplierAsync(SupplierDto newSupplierDto);

        Task<SupplierDto?> GetSupplierByIdAsync(int id);
        // CRUD: UPDATE (NUEVO)
        Task<bool> UpdateSupplierAsync(SupplierDto updatedSupplierDto);

        Task<bool> DeleteSupplierAsync(int id);

    }
}
