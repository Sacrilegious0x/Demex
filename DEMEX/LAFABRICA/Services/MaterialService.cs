// En LAFABRICA/Services/MaterialService.cs
using AspNetCoreGeneratedDocument;
using LAFABRICA.Data.DB;
using LAFABRICA.Models.AuxiliarDTOS;
using LAFABRICA.Models.Interface;
using Microsoft.AspNetCore.Identity;
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


        public async Task<Material> CreateMaterial(Material material, int suppleirId, int? 
            quantityToMaterialSupplier, 
            int? minimumQuantity)
        {
            using var _context = _contextFactory.CreateDbContext();

            if (material.PhotoUrl == null)
            {
                material.PhotoUrl = "default";
            }

            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            await addInMaterial_Supplier(material.Id, suppleirId, quantityToMaterialSupplier);

            await addInInventory(material.Id,minimumQuantity);
            

            return material;
        }

        public async Task DeleteMaterial(int id)
        {

            using var _context = _contextFactory.CreateDbContext();

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
            using var context = _contextFactory.CreateDbContext();
            return await context.Materials.Where(m => m.IsActive == true).ToListAsync();
        }

        public async Task<Material>? GetMaterialById(int id)
        {
            using var _context = _contextFactory.CreateDbContext();

            var material = await _context.Materials.FindAsync(id);
            return material;
        }



        public async Task<Material> UpdateMaterial(int id, Material material)
        {

            using var _context = _contextFactory.CreateDbContext();

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

            using var _context = _contextFactory.CreateDbContext();

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

            using var _context = _contextFactory.CreateDbContext();

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

                // Agregar la nueva entidad al DbSet
                _context.MaterialSuppliers.Add(newMaterialSupplier);

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // Devolver true si no revento
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

            using var _context = _contextFactory.CreateDbContext();
            try
            {
                // 1. Crear una nueva instancia de la entidad de enlace
                var newMaterialInInventory = new Inventory
                {
                    // Las dos claves primarias/foráneas
                    MaterialId = materialId,
                    MinimunQuantity = (int)minimumQuantity,
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

        // Este es un fragmento de la clase MaterialService
        public async Task<MaterialSupplierInventoryDto?> GetMaterialDetailByIds(int materialId, int supplierId)
        {
            using var _context = _contextFactory.CreateDbContext();

            var query = _context.Materials
                // Filtra por el MaterialId inmediatamente 
                .Where(m => m.Id == materialId && (bool)m.IsActive)

                // Une MATERIAL con MATERIAL_SUPPLIER
                .SelectMany(m => m.MaterialSuppliers.Where(ms => ms.SupplierId == supplierId), (m, ms) => new { m, ms })

                // 2. Une el resultado con INVENTORY
                .Join(_context.Inventories,
                    anon => anon.m.Id,
                    i => i.MaterialId,
                    (anon, i) => new { anon.m, anon.ms, i })

                // Filtramos también en la relación Inventory por si acaso (aunque Inventory está 1:1 con Material)
                // Y lo más importante, filtramos por el SupplierId que viene en el SelectMany
                .Where(dto => dto.ms.SupplierId == supplierId && dto.m.Id == materialId)

                // Construye el DTO
                .Select(dto => new MaterialSupplierInventoryDto
                {
                    MaterialId = dto.m.Id,
                    SupplierId = dto.ms.SupplierId,
                    MaterialName = dto.m.Name,
                    SupplierName = dto.ms.Supplier.Name, // Navegación implícita a Supplier
                    Quantity = dto.ms.Quantity,         // Cantidad de Material_Supplier (Stock Actual)
                    Unit = dto.m.Unit,
                    MinimumQuantity = dto.i.MinimunQuantity, // Cantidad minima de Inventario
                    PricePurchase = dto.m.PricePurchase,
                    photoUrl = dto.m.PhotoUrl
                });

            // Usa FirstOrDefaultAsync ya que solo esperamos un resultado
            return await query.FirstOrDefaultAsync();
        }

        // Este es un fragmento de la clase MaterialService
        public async Task UpdateMaterial(MaterialSupplierInventoryDto dto)
        {
            using var _context = _contextFactory.CreateDbContext();
            // Inicia una transacción si tu contexto lo permite (buena práctica para actualizar múltiples tablas)
            // using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. ACTUALIZAR ENTIDAD MATERIAL
                var material = await _context.Materials
                    .FirstOrDefaultAsync(m => m.Id == dto.MaterialId);

                if (material == null)
                {
                    throw new InvalidOperationException($"Material con ID {dto.MaterialId} no encontrado.");
                }

                // Se actualizan campos del Material

                material.Name = dto.MaterialName;
                material.Unit = dto.Unit;
                material.PricePurchase = dto.PricePurchase;
                material.PhotoUrl = dto.photoUrl; // Se actualiza si hay una URL nueva
                                                  // No es necesario llamar a _context.Update(material) si EF Core está siguiendo la entidad

                // ACTUALIZAR ENTIDAD INVENTORY
                // Inventory está relacionado 1:1 o 1:N con Material
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.MaterialId == dto.MaterialId);

                if (inventory == null)
                {
                    throw new InvalidOperationException($"Registro de Inventario para Material ID {dto.MaterialId} no encontrado.");
                }

                // Se actualiza el campo de Inventario
                inventory.MinimunQuantity = (int)dto.MinimumQuantity;


                // ACTUALIZAR ENTIDAD MATERIAL_SUPPLIER (el Stock Actual)
                var materialSupplier = await _context.MaterialSuppliers
                    .FirstOrDefaultAsync(ms => ms.MaterialId == dto.MaterialId && ms.SupplierId == dto.SupplierId);

                if (materialSupplier == null)
                {
                    throw new InvalidOperationException($"Relación Material-Proveedor para IDs {dto.MaterialId} y {dto.SupplierId} no encontrada.");
                }

                // Se actualiza el stock actual de esa relación
                materialSupplier.Quantity = dto.Quantity;


                // GUARDAR TODOS LOS CAMBIOS
                await _context.SaveChangesAsync();
                // await transaction.CommitAsync();

            }
            catch (Exception)
            {
                // await transaction.RollbackAsync();
                throw; // Relanza la excepción para que el Blazor Component la capture y muestre el Toast
            }
        }



    }


}