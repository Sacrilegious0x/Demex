// En LAFABRICA/Services/MaterialService.cs
using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;

namespace LAFABRICA.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly AppDbContext _context;
        public MaterialService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Material>> GetAllMaterials()
        {
            return await _context.Materials.ToListAsync();
        }
    }
}