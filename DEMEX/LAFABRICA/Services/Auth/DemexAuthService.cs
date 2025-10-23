using LAFABRICA.Data.DB;
using Microsoft.EntityFrameworkCore;


namespace LAFABRICA.Services.Auth
{
    public class DemexAuthService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public DemexAuthService(IDbContextFactory<AppDbContext> context)
        {
            _contextFactory = context;
        }


        public async Task<User?> Login(string email, string password)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users
                .Include(u => u.Rol)
                .ThenInclude(r => r.RolePermissions)
                .FirstOrDefaultAsync(u =>
                    u.Email == email &&
                    u.Password == password &&
                    u.IsActive == 1);
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users
                .Include(u => u.Rol)
                .ThenInclude(r => r.RolePermissions)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
