using LAFABRICA.Data.DB;

namespace LAFABRICA.Models.Interface
{
    public interface IRolePermissonService
    {

        Task<IEnumerable<RolePermission>> GetAllRols();
        Task<RolePermission?> GetById(int id);
        Task<RolePermission> Create(RolePermission rolper);
        Task<RolePermission> Update(int id, RolePermission rolper);
        Task Delete(int id);
    }
}
