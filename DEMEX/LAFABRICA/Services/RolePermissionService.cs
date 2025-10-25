using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;

namespace LAFABRICA.Services
{
    public class RolePermissionService : IRolePermissonService
    {

        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public RolePermissionService(IDbContextFactory<AppDbContext> context)
        {
            _contextFactory = context;
        }
        public Task<RolePermission> Create(RolePermission rolper)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RolePermission>> GetAllRols()
        {
            throw new NotImplementedException();
        }

        public Task<RolePermission?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<RolePermission> Update(int id, RolePermission rolper)
        {
            using var context = _contextFactory.CreateDbContext();

            var existing = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == id && rp.Module == rolper.Module);

            if (existing == null)
                throw new Exception("Permiso no encontrado.");

            existing.Canview = rolper.Canview;
            existing.Cancreate = rolper.Cancreate;
            existing.Canedit = rolper.Canedit;
            existing.Candelete = rolper.Candelete;

            context.RolePermissions.Update(existing);
            await context.SaveChangesAsync();
            return existing;
        }
    }
}
