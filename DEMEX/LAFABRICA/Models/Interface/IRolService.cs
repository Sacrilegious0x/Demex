using LAFABRICA.Data.DB;

namespace LAFABRICA.Models.Interface
{
    public interface IRolService
    {
        Task<IEnumerable<Rol>> GetAllRols();
        Task<Rol?> GetById(int id);
        Task<Rol> Create(Rol rol);
        Task<Rol> Update(int id, Rol rol);
        Task Delete(int id);
    }
}
