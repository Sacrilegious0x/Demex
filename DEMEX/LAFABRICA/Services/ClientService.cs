using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;
using LAFABRICA.Data.DB;
using System.Data;

namespace LAFABRICA.Services
{
    public class ClientService: IClientService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ClientService(IDbContextFactory<AppDbContext> contex)
        {
            _contextFactory = contex;
        }

        public async Task<Client> Create(Client client)
        {

            using var context = _contextFactory.CreateDbContext();
            bool existEmail = await context.Clients.AnyAsync(c => c.Email == client.Email && c.IsActive == 1);

            if (existEmail)
            {
                throw new InvalidOperationException("El correo ya esta registrado");
            }
            bool existPhone = await context.Clients.AnyAsync(c => c.PhoneNumber == client.PhoneNumber && c.IsActive == 1);
            if (existPhone)
            {
                throw new InvalidOperationException("El contacto de la empresa ya esta registrado");
            }
            bool existManagerPhone = await context.Clients.AnyAsync(c => c.ManagerPhoneNumber == client.ManagerPhoneNumber && c.IsActive == 1);
            if (existManagerPhone)
            {
                throw new InvalidOperationException("El contacto del encargado ya esta registrado");
            }

            context.Clients.Add(client);
            await context.SaveChangesAsync();
            return client;
        }

        public async Task Delete(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var client = await context.Clients.FindAsync(id);
            if (client == null)
                throw new KeyNotFoundException($"Cliente con id {id} no encontrado");
            client.IsActive = 0;
            context.Clients.Update(client);
            //_contex.Clients.Remove(client);
            
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Client>> GetAllClient()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Clients
                .Where(c => c.IsActive == 1)
                .ToListAsync();
        }

        public async Task<Client?> GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var client = await context.Clients.FindAsync(id);
            return client;
        }

        public async Task<Client> Update(int id, Client client)
        {
            using var context = _contextFactory.CreateDbContext();
            var oldClient = await context.Clients.FindAsync(id);
            if (oldClient == null)
                throw new KeyNotFoundException($"El cliente con el id {id} no encontrado");
            bool existEmail = await context.Clients.AnyAsync(c => c.Id != id && c.Email == client.Email && c.IsActive == 1);
            if (existEmail)
            {
                throw new InvalidOperationException("El correo ya esta registrado");
            }
            bool existPhone = await context.Clients.AnyAsync(c => c.Id != id && c.PhoneNumber == client.PhoneNumber && c.IsActive == 1);
            if (existPhone)
            {
                throw new InvalidOperationException("El contacto de la empresa ya esta registrado");
            }
            bool existManagerPhone = await context.Clients.AnyAsync(c => c.Id != id && c.ManagerPhoneNumber == client.ManagerPhoneNumber && c.IsActive == 1);
            if (existManagerPhone)
            {
                throw new InvalidOperationException("El contacto del encargado ya esta registrado");
            }
            context.Entry(oldClient).CurrentValues.SetValues(client);
            await context.SaveChangesAsync();
            return client;
        }
    }
}
