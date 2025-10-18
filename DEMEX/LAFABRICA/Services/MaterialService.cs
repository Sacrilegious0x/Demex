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

        public async Task<Material> CreateMaterial(Material material)
        {
           _context.Materials.Add(material);
            await _context.SaveChangesAsync();
            return material;
        }

        public async Task DeleteMaterial(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                throw new KeyNotFoundException($"Material con id {id} no encontrado en create Material");
                
            }
            material.IsActive = false;
            _context.Materials.Update(material);

            await _context.SaveChangesAsync();
            
        }

        public async Task<IEnumerable<Material>> GetAllMaterials()
        {
            return await _context.Materials.ToListAsync();
        }

        public async Task<Material>? GetMaterialById(int id)
        {
            var material  = await _context.Materials.FindAsync(id);
            return material;
        }

        public async Task<Material> UpdateMaterial(int id, Material material)
        {
            var oldMaterial = await _context.Materials.FindAsync(id);

            if(oldMaterial == null)
            {
                throw new KeyNotFoundException($"El material con el id {id} no encontrado en UpdateMaterial");
            }
            _context.Entry(oldMaterial).CurrentValues.SetValues(material);
            await _context.SaveChangesAsync();
            return material;
        }

    }
}