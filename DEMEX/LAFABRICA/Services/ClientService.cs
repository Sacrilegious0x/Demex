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


            bool existEmail = await _contex.Clients.AnyAsync(c => c.Email == client.Email);
            if (existEmail)
            {
                throw new InvalidOperationException("El correo ya esta registrado");
            }
            bool existPhone = await _contex.Clients.AnyAsync(c => c.PhoneNumber == client.PhoneNumber);
            if (existPhone)
            {
                throw new InvalidOperationException("El contacto de la empresa ya esta registrado");
            }
            bool existManagerPhone = await _contex.Clients.AnyAsync(c => c.ManagerPhoneNumber == client.ManagerPhoneNumber);
            if (existManagerPhone)
            {
                throw new InvalidOperationException("El contacto del encargado ya esta registrado");
            }

            _contex.Clients.Add(client);
            await _contex.SaveChangesAsync();
            return client;
        }

        public async Task Delete(int id)
        {
            var client = await _contex.Clients.FindAsync(id);
            if (client == null)
                throw new KeyNotFoundException($"Cliente con id {id} no encontrado");
            client.IsActive = 0;
            _contex.Clients.Update(client);
            //_contex.Clients.Remove(client);
            
            await _contex.SaveChangesAsync();
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
            bool existEmail = await _contex.Clients.AnyAsync(c => c.Id != id && c.Email == client.Email);
            if (existEmail)
            {
                throw new InvalidOperationException("El correo ya esta registrado");
            }
            bool existPhone = await _contex.Clients.AnyAsync(c => c.Id != id && c.PhoneNumber == client.PhoneNumber);
            if (existPhone)
            {
                throw new InvalidOperationException("El contacto de la empresa ya esta registrado");
            }
            bool existManagerPhone = await _contex.Clients.AnyAsync(c => c.Id != id && c.ManagerPhoneNumber == client.ManagerPhoneNumber);
            if (existManagerPhone)
            {
                throw new InvalidOperationException("El contacto del encargado ya esta registrado");
            }
            _contex.Entry(oldClient).CurrentValues.SetValues(client);
            await _contex.SaveChangesAsync();
            return client;
        }
    }
}
