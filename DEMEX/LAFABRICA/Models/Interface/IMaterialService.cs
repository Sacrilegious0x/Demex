using LAFABRICA.Data.DB;

namespace LAFABRICA.Models.Interface
{
    public interface IMaterialService
    {
        Task<IEnumerable<Material>> GetAllMaterials();

        Task<Material>? GetMaterialById(int id);

        Task<Material> CreateMaterial(Material material);

        Task<Material> UpdateMaterial(int id, Material material);   

        Task DeleteMaterial(int id);


    }
}