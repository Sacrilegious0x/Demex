using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LAFABRICA.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _context.Products
                .Where(p => p.IsActive == 1) // <-- Filtrar sólo activos
                .Include(p => p.ProductMaterials)
                .ThenInclude(pm => pm.Material)
                .ToListAsync();
        }

        public async Task<Product?> GetById(int id)
        {
            return await _context.Products
                .Include(p => p.ProductMaterials)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> Create(Product product)
        {
            var materialIds = product.ProductMaterials?
                                .Select(pm => pm.MaterialId)
                                .Where(id => id != 0)
                                .Distinct()
                                .ToList()
                              ?? new List<int>();

            product.ProductMaterials = materialIds
                .Select(mid => new ProductMaterial { MaterialId = mid })
                .ToList();

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task Update(int id, Product updatedProduct)
        {
            var existing = await _context.Products
                .Include(p => p.ProductMaterials)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existing == null)
                throw new KeyNotFoundException($"Producto con id {id} no encontrado.");

            // Actualizar campos escalares
            existing.Name = updatedProduct.Name;
            existing.Category = updatedProduct.Category;
            existing.Description = updatedProduct.Description;
            existing.PriceBase = updatedProduct.PriceBase;
            existing.IsCustom = updatedProduct.IsCustom;
            existing.Complexity = updatedProduct.Complexity;
            existing.PhotoUrl = updatedProduct.PhotoUrl;
            existing.IsActive = updatedProduct.IsActive;

            // Sincronizar la tabla intermedia ProductMaterials
            var incomingIds = (updatedProduct.ProductMaterials ?? Enumerable.Empty<ProductMaterial>())
                                .Select(pm => pm.MaterialId)
                                .Where(mid => mid != 0)
                                .ToHashSet();

            var existingList = existing.ProductMaterials.ToList();
            var existingIds = existingList.Select(pm => pm.MaterialId).ToHashSet();

            // Remover relaciones que ya no existen
            var toRemove = existingList.Where(pm => !incomingIds.Contains(pm.MaterialId)).ToList();
            if (toRemove.Any())
            {
                _context.ProductMaterials.RemoveRange(toRemove);
            }

            // Añadir nuevas relaciones
            var toAddIds = incomingIds.Except(existingIds);
            foreach (var mid in toAddIds)
            {
                existing.ProductMaterials.Add(new ProductMaterial
                {
                    ProductId = existing.Id,
                    MaterialId = mid
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            product.IsActive = 0;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}