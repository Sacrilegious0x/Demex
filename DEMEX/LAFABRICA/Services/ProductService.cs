using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;

namespace LAFABRICA.Services
{
    public class ProductService : IProductService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ProductService(IDbContextFactory<AppDbContext> context)
        {
            _contextFactory = context;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Products
                .Where(p => p.IsActive == 1)
                .Include(p => p.ProductMaterials)
                    .ThenInclude(pm => pm.Material) 
                .ToListAsync();
        }

        
        public async Task<Product?> GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Products
                .Include(p => p.ProductMaterials)
                    .ThenInclude(pm => pm.Material) 
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive == 1); 
        }
       

        public async Task<Product> Create(Product product)
        {
            using var context = _contextFactory.CreateDbContext();
            var materialIds = product.ProductMaterials?
                                .Select(pm => pm.MaterialId)
                                .Where(id => id != 0)
                                .Distinct()
                                .ToList()
                               ?? new List<int>();

            // Limpiar la lista original y re-crearla solo con IDs antes de añadir
            product.ProductMaterials.Clear();
            product.ProductMaterials = materialIds
                .Select(mid => new ProductMaterial { MaterialId = mid })
                .ToList();

            context.Products.Add(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task Update(int id, Product updatedProduct)
        {
            using var context = _contextFactory.CreateDbContext();
            var existing = await context.Products
                .Include(p => p.ProductMaterials)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existing == null)
                throw new KeyNotFoundException($"Producto con id {id} no encontrado.");

            // Actualizar campos escalares
            context.Entry(existing).CurrentValues.SetValues(updatedProduct);
            // Reasegurar que el ID no cambie por SetValues
            existing.Id = id;

            // Sincronizar ProductMaterials
            var incomingIds = (updatedProduct.ProductMaterials ?? Enumerable.Empty<ProductMaterial>())
                                .Select(pm => pm.MaterialId)
                                .Where(mid => mid != 0)
                                .ToHashSet();

            var existingList = existing.ProductMaterials.ToList(); // Materializar

            // Remover
            var toRemove = existingList.Where(pm => !incomingIds.Contains(pm.MaterialId)).ToList();
            if (toRemove.Any())
            {
                context.ProductMaterials.RemoveRange(toRemove); 
            }

            // Añadir
            var existingIds = existingList.Select(pm => pm.MaterialId).ToHashSet();
            var toAddIds = incomingIds.Except(existingIds);
            foreach (var mid in toAddIds)
            {
                // Añadir directamente al contexto es más seguro que a la colección
                context.ProductMaterials.Add(new ProductMaterial
                {
                    ProductId = existing.Id, // Usar el ID del producto existente
                    MaterialId = mid
                });
            }

            await context.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var product = await context.Products.FindAsync(id);
            if (product == null)
                return false;

            product.IsActive = 0; // Borrado lógico
            context.Products.Update(product);
            await context.SaveChangesAsync();
            return true;
        }
    }
}