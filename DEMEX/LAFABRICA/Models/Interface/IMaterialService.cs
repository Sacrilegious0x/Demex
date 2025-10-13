using LAFABRICA.Data.DB;

namespace LAFABRICA.Models.Interface
{
    public interface IMaterialService
    {
        Task<List<Material>> GetAllMaterials();
    }
}