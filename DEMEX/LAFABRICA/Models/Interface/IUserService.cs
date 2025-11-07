using LAFABRICA.Data.DB;

namespace LAFABRICA.Models.Interface
{
    public interface IUserService
    {

        Task<IEnumerable<User>> GetAllUsers();
        Task<User?> GetById(int id);
        Task<User> Create(User user);
        Task<User> Update(int id, User user);
        Task Delete(int id);
    }
}
