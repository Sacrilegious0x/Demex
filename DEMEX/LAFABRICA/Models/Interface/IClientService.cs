using LAFABRICA.Data.DB;
using LAFABRICA.Models.AuxiliarDTOS;

namespace LAFABRICA.Models.Interface
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAllClient();
        Task<Client?> GetById(int id);
        Task<Client> Create(Client client);
        Task<Client> Update(int id, Client client);
        Task Delete(int id);
       Task<ClientOrdersDTO?> GetClientWithCompletedOrders(int id);
        Task UpdateFrequentStatus(int clientId);
    }
}
