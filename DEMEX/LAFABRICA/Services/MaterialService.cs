// En LAFABRICA/Services/MaterialService.cs
using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace LAFABRICA.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public MaterialService(IDbContextFactory<AppDbContext> context)
        {
            _contextFactory = context;
        }

        public async Task<List<Material>> GetAllMaterials()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Materials.ToListAsync();
        }
    }
}