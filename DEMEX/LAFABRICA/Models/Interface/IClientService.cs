using LAFABRICA.Data.DB;

namespace LAFABRICA.Models.Interface
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAllClient();
        Task<Client?> GetById(int id);
        Task<Client> Create(Client client);
        Task<Client> Update(int id, Client client);
        Task<bool> Delete(int id);

    }
}
