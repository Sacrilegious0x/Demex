using LAFABRICA.Data.DB;
using LAFABRICA.Models.AuxiliarDTOS;

namespace LAFABRICA.Models.Interface
{
    public interface IMaterialService
    {
        Task<IEnumerable<Material>> GetAllMaterials();

        Task<Material>? GetMaterialById(int id);

        Task<Material> CreateMaterial(Material material, int supplierId,int? quantity, int? minimumQuantity);

        Task<Material> UpdateMaterial(int id, Material material);   

        Task DeleteMaterial(int id);

        Task<IEnumerable<MaterialSupplierInventoryDto>> GetMaterialDetails();


    }
}