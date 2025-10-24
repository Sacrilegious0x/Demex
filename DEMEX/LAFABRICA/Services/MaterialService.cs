// En LAFABRICA/Services/MaterialService.cs
using AspNetCoreGeneratedDocument;
using LAFABRICA.Data.DB;
using LAFABRICA.Models.AuxiliarDTOS;
using LAFABRICA.Models.Interface;
using Microsoft.AspNetCore.Identity;
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

        public async Task<Material> CreateMaterial(Material material, int suppleirId, int? 
            quantityToMaterialSupplier, 
            int? minimumQuantity)
        {
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            await addInMaterial_Supplier(material.Id, suppleirId, quantityToMaterialSupplier);

            await addInInventory(material.Id,minimumQuantity);
            

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
            var material = await _context.Materials.FindAsync(id);
            return material;
        }



        public async Task<Material> UpdateMaterial(int id, Material material)
        {
            var oldMaterial = await _context.Materials.FindAsync(id);

            if (oldMaterial == null)
            {
                throw new KeyNotFoundException($"El material con el id {id} no encontrado en UpdateMaterial");
            }
            _context.Entry(oldMaterial).CurrentValues.SetValues(material);
            await _context.SaveChangesAsync();
            return material;
        }


        public async Task<IEnumerable<MaterialSupplierInventoryDto>> GetMaterialDetails()
        {
            var queryMaterialsToInventory = _context.Materials
                // 1. Une MATERIAL con MATERIAL_SUPPLIER (N:M)
                // Se usa SelectMany para aplanar la relación y generar filas por cada Material-Supplier
                .Where(m => (bool)m.IsActive)
                .SelectMany(m => m.MaterialSuppliers, (m, ms) => new { m, ms })
                // Une el resultado con INVENTORY 
                .Join(_context.Inventories,
                      anon => anon.m.Id,         // Clave de Material
                      i => i.MaterialId,         // Clave de Inventario
                      (anon, i) => new { anon.m, anon.ms, i })

                // se construye el DTO
                .Select(dto => new MaterialSupplierInventoryDto
                {
                    MaterialId = dto.m.Id,
                    SupplierId = dto.ms.SupplierId,
                    MaterialName = dto.m.Name,
                    SupplierName = dto.ms.Supplier.Name, // Navegación implícita a Supplier
                    Quantity = dto.ms.Quantity, //Cantidad de la tabla material_Supplier
                    Unit = dto.m.Unit,
                    MinimumQuantity = dto.i.MinimunQuantity, // Cantidad minima del inventario
                    PricePurchase = dto.m.PricePurchase,
                    photoUrl = dto.m.PhotoUrl
                });

            return await queryMaterialsToInventory.ToListAsync();
        }

        private async Task<bool> addInMaterial_Supplier(int materialId, int supplierId, int? quantity)
        {

            try
            {
                // 1. Crear una nueva instancia de la entidad de enlace
                var newMaterialSupplier = new MaterialSupplier
                {
                    // Las dos claves primarias/foráneas
                    MaterialId = materialId,
                    SupplierId = supplierId,

                    // La cantidad (que representa la cantidad de suministro o pedido mínimo)
                    Quantity = quantity
                };

                // 2. Agregar la nueva entidad al DbSet
                _context.MaterialSuppliers.Add(newMaterialSupplier);

                // 3. Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // 4. Devolver true si la operación fue exitosa
                return true;
            }
            catch (Exception ex)
            {
                // Opcional: Loguear el error (recomendado)
                // Console.WriteLine($"Error al agregar MaterialSupplier: {ex.Message}");
                Console.WriteLine(ex.ToString());
                // Devolver false si la operación falló
                return false;
            }
        }

        private async Task<bool> addInInventory(int materialId, int? minimumQuantity)
        {
            try
            {
                // 1. Crear una nueva instancia de la entidad de enlace
                var newMaterialInInventory = new Inventory
                {
                    // Las dos claves primarias/foráneas
                    MaterialId = materialId,
                    MinimunQuantity = minimumQuantity,
                    State = "default",
                    Quantity = 1

                };

                // 2. Agregar la nueva entidad al DbSet
                _context.Inventories.Add(newMaterialInInventory);

                // 3. Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // 4. Devolver true si la operación fue exitosa
                return true;
            }
            catch (Exception ex)
            {
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }

            }

        }
    }
}