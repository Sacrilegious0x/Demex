using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LAFABRICA.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public UserService(IDbContextFactory<AppDbContext> context)
        {
            _contextFactory = context;
        }


        public async Task<User> Create(User user)
        {
            using var context = _contextFactory.CreateDbContext();


            bool existEmail = await context.Users
                .AnyAsync(u => u.Email == user.Email && u.IsActive == 1);
            if (existEmail)
                throw new InvalidOperationException("El correo ya está registrado.");

            bool existIdentification = await context.Users
                .AnyAsync(u => u.Identification == user.Identification && u.IsActive == 1);
            if (existIdentification)
                throw new InvalidOperationException("La identificación ya está registrada.");



            var rol = await context.Rols.FindAsync(user.RolId);
            if (rol == null)
                throw new InvalidOperationException("El rol seleccionado no existe.");

            user.UserType = rol.Name;


            string randomPassword = GenerateRandomPassword();
            user.Password = BCrypt.Net.BCrypt.HashPassword(randomPassword);
            user.IsActive = 1;

            context.Users.Add(user);
            await context.SaveChangesAsync();

            // 🔹 (Opcional) Enviar correo con la contraseña temporal
            // await emailService.SendAsync(user.Email, "Cuenta creada", $"Tu contraseña temporal es: {randomPassword}");

            return user;
        }


        public async Task Delete(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FindAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"Usuario con id {id} no encontrado.");

            user.IsActive = 0;
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }


        public async Task<IEnumerable<User>> GetAllUsers()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users
                .Include(u => u.Rol)
                .Where(u => u.IsActive == 1)
                .ToListAsync();
        }


        public async Task<User?> GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == id);
        }


        public async Task<User> Update(int id, User user)
        {
            using var context = _contextFactory.CreateDbContext();

            var existingUser = await context.Users.FindAsync(id);
            if (existingUser == null)
                throw new KeyNotFoundException($"El usuario con id {id} no existe.");


            bool existEmail = await context.Users
                .AnyAsync(u => u.Id != id && u.Email == user.Email && u.IsActive == 1);
            if (existEmail)
                throw new InvalidOperationException("El correo ya está registrado.");

            bool existIdentification = await context.Users
                .AnyAsync(u => u.Id != id && u.Identification == user.Identification && u.IsActive == 1);
            if (existIdentification)
                throw new InvalidOperationException("La identificación ya está registrada.");

            var rol = await context.Rols.FindAsync(user.RolId);
            if (rol == null)
                throw new InvalidOperationException("El rol seleccionado no existe.");
            user.UserType = rol.Name;


            user.Password = existingUser.Password;

            context.Entry(existingUser).CurrentValues.SetValues(user);
            await context.SaveChangesAsync();

            return user;
        }


        private string GenerateRandomPassword()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
