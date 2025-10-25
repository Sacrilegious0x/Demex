using LAFABRICA.Data.DB;
using LAFABRICA.Models.AuxiliarDTOS;

namespace LAFABRICA.Models.Interface
{
    public interface IMaterialService
    {
        Task<IEnumerable<Material>> GetAllMaterials();

        Task<Material>? GetMaterialById(int id);

        Task<Material> CreateMaterial(Material material, int supplierId,int? quantity, int? minimumQuantity);

        Task UpdateMaterial(MaterialSupplierInventoryDto dto);

        Task DeleteMaterial(int id);

        Task<IEnumerable<MaterialSupplierInventoryDto>> GetMaterialDetails();

        Task<MaterialSupplierInventoryDto?> GetMaterialDetailByIds(int materialId, int supplierId);
    }
}