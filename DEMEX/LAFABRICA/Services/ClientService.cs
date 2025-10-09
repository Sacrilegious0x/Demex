using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;
using LAFABRICA.Data.DB;
using System.Data;

namespace LAFABRICA.Services
{
    public class ClientService: IClientService
    {
        private readonly AppDbContext _contex;

        public ClientService(AppDbContext contex)
        {
            _contex = contex;
        }

        public async Task<Client> Create(Client client)
        {
            _contex.Clients.Add(client);
            await _contex.SaveChangesAsync();
            return client;
        }

        public async Task<bool> Delete(int id)
        {
            var client = await _contex.Clients.FindAsync(id);
            if (client == null)
                return false;

            _contex.Clients.Remove(client);
            await _contex.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Client>> GetAllClient()
        {
            return await _contex.Clients.ToListAsync();
        }

        public async Task<Client?> GetById(int id)
        {
            var client = await _contex.Clients.FindAsync(id);
            return client;
        }

        public async Task<Client> Update(int id, Client client)
        {
            var oldClient = await _contex.Clients.FindAsync(id);
            if (oldClient == null)
                throw new KeyNotFoundException($"El cliente con el id {id} no encontrado");

            _contex.Entry(oldClient).CurrentValues.SetValues(client);
            await _contex.SaveChangesAsync();
            return client;
        }
    }
}
