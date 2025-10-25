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
        private readonly IDbContextFactory<AppDbContext> _contextFactory; // Conexion a la db
        // Nota: readonly = Final en java 

        public ProductService(IDbContextFactory<AppDbContext> context)
        {
            _contextFactory = context;
        }

        // Nota: IEnmerable es similar a un List<T> pero no se puede editar la lista 
        public async Task<IEnumerable<Product>> GetAllProducts()   // Nota: Task = Future<T> en java, basicamente un promesa de lo que se va a enviar al finalizar 
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Products   // Nota: Debido a este async y await se permite manejar esas pausas sin bloquear el programa
                .Where(p => p.IsActive == 1) // Filtrar sólo activos
                .Include(p => p.ProductMaterials)
                .ThenInclude(pm => pm.Material)
                .ToListAsync();
        }

        public async Task<Product?> GetById(int id)   // Nota: "?" = Significa que se puede devolver un producto a como se puede devolver un NULL
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Products
                .Include(p => p.ProductMaterials)   // Nota: "Include" = sirve para traer informacion de tablas relacionadas a la identidad
                .FirstOrDefaultAsync(p => p.Id == id);   // Nota: "FirstOrDefaultAsync" = Devuelve el primero en cumplir con la condicion
            
            // Nota: Esa: "p" solo es el nombre del elemento, puede ser cualquier caracter o palabra, solo es una forma de referirse a los elementos de la coleccion
        }       

        public async Task<Product> Create(Product product)
        {
            using var context = _contextFactory.CreateDbContext();
            var materialIds = product.ProductMaterials?   // Nta: Var = "Deja que el compilador averigüe el tipo de dato por mí"
                                .Select(pm => pm.MaterialId)
                                .Where(id => id != 0)
                                .Distinct()
                                .ToList()
                              ?? new List<int>();

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
                context.ProductMaterials.RemoveRange(toRemove);
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

            await context.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var product = await context.Products.FindAsync(id);
            if (product == null)
                return false;

            product.IsActive = 0;
            context.Products.Update(product);
            await context.SaveChangesAsync();
            return true;
        }
    }
}