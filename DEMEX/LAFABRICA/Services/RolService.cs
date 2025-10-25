using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;


namespace LAFABRICA.Services
{
    public class RolService : IRolService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public RolService(IDbContextFactory<AppDbContext> context)
        {
            _contextFactory = context;
        }
        public async Task<Rol> Create(Rol rol)
        {
            using var context = _contextFactory.CreateDbContext();

            rol.IsActive = 1;

            rol.Name = rol.Name?.Trim().ToUpper() ?? string.Empty;
            rol.RolePermissions = DefaultPermissions
                .Select(p => new RolePermission
                {
                    Module = p.Module,
                    Canview = p.Canview,
                    Cancreate = p.Cancreate,
                    Candelete = p.Candelete,
                    Canedit = p.Canedit
                })
                .ToList();

            context.Rols.Add(rol);
            await context.SaveChangesAsync();

            return rol;
        }

        public async Task Delete(int id)
        {
            using var context = _contextFactory.CreateDbContext();

            var rol = await context.Rols.FirstOrDefaultAsync(r => r.Id == id);

            if (rol == null)
                throw new Exception("El rol no existe.");

            rol.IsActive = 0;
            context.Rols.Update(rol);

            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Rol>> GetAllRols()
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Rols
                .Include(r => r.RolePermissions)
                .Where(r => r.IsActive == 1)
                .ToListAsync();
        }

        public async Task<Rol?> GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Rols
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task<Rol> Update(int id, Rol rol)
        {
            throw new NotImplementedException();
        }

        private static readonly List<RolePermission> DefaultPermissions = new()
        {
            new() { Module = "CLIENTS", Canview = true, Cancreate = true, Candelete = true, Canedit = true },
            new() { Module = "CLIENTS_PAYMENTS", Canview = true, Cancreate = false, Candelete = false, Canedit = false },
            new() { Module = "INVENTORY", Canview = true, Cancreate = true, Candelete = true, Canedit = true },
            new() { Module = "MATERIAL", Canview = true , Cancreate = true, Candelete = true, Canedit = true },
            new() { Module = "ORDERS", Canview = true, Cancreate = true, Candelete = true, Canedit = true },
            new() { Module = "PRODUCTS", Canview = true, Cancreate = true, Candelete = true, Canedit = true },
            new() { Module = "REPORTS", Canview = true, Cancreate = true, Candelete = true, Canedit = true },
            new() { Module = "ROLS", Canview = true, Cancreate = true, Candelete = true, Canedit = true },
            new() { Module = "SUPPLIERS", Canview = true, Cancreate = true, Candelete = true  , Canedit = true },
            new() { Module = "USERS", Canview = true, Cancreate = true, Candelete = true, Canedit = true },
            new() { Module = "WORKERS_PAYMENTS", Canview = true, Cancreate = true, Candelete = true, Canedit = true },
            new() { Module = "TOURS", Canview = true, Cancreate = true, Candelete = true, Canedit = true }

        };
    }
}
