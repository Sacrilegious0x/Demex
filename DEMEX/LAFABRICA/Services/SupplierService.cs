using LAFABRICA.Data.DB;
using LAFABRICA.Models.AuxiliarDTOS;
using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LAFABRICA.Services
{
    public class SupplierService : ISupplierService
    {

        private readonly IDbContextFactory<AppDbContext> _contextFactory;


        public SupplierService(IDbContextFactory<AppDbContext> context)
        {
            _contextFactory = context;
        }

        public async Task<List<SupplierDto>> GetSuppliersAsync()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var suppliers = await context.Suppliers
                .Where(s => s.IsActive) // FILTRO
                .ToListAsync();

                // Mapea la entidad a la DTO (Si el modelo Supplier no es igual a SupplierDto)
                // Si el modelo Supplier de la BD y SupplierDto son idénticos se puede usar el DTO y usar la entidad,
                // pero segun la documentacion disque es buena practica hacerlo asi, la verdad yo no lo veo asi , pero bueno confiemos en quienes diseniaron el lenguaje XD

                var supplierDtos = suppliers.Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Address = s.Address,
                    Phone = s.Phone,
                    Email = s.Email,
                    DateLastPurchase = s.DateLastPurchase,
                    Notes = s.Notes,
                    IsActive = s.IsActive,
                }).ToList();

                return supplierDtos;
            }
            catch (Exception ex)
            {
                // Manejo de errores de base de datos
                Console.WriteLine($"Error al obtener proveedores desde la BD: {ex.Message}");
                // En un servicio in-process, es mejor lanzar la excepción o usar un logger pero aqui retornamos una lista porque somos unos duros

                return new List<SupplierDto>();
            }




        }

        public async Task<SupplierDto> AddSupplierAsync(SupplierDto newSupplierDto)
        {
            // Mapear DTO a la Entidad de EF Core (Supplier)
            using var context = _contextFactory.CreateDbContext();
            var supplierEntity = new Supplier
            {
                // El Id debe ser 0 o ignorarse ya que es autoincremental
                Name = newSupplierDto.Name,
                Address = newSupplierDto.Address,
                Phone = newSupplierDto.Phone,
                Email = newSupplierDto.Email,
                DateLastPurchase = newSupplierDto.DateLastPurchase,
                Notes = newSupplierDto.Notes,
                IsActive = true


            };

            bool existEmail = await context.Suppliers
                           .AnyAsync(s => s.Email == supplierEntity.Email && s.IsActive == true);
            if (existEmail)
            {
                throw new InvalidOperationException("El correo ya está registrado por un proveedor activo");
            }

            bool existPhone = await context.Suppliers
                           .AnyAsync(s => s.Phone == supplierEntity.Phone && s.IsActive == true);
            if (existPhone)
            {
                throw new InvalidOperationException("El contacto del proveedor ya está registrado por un proveedor activo");
            }

            context.Suppliers.Add(supplierEntity);

            //  Guardar los cambios en la base de datos
            await context.SaveChangesAsync();


            return new SupplierDto
            {
                Id = supplierEntity.Id, // ID Autoincremental generado por la BD
                Name = supplierEntity.Name,
                Address = supplierEntity.Address,
                Phone = supplierEntity.Phone,
                Email = supplierEntity.Email,
                DateLastPurchase = supplierEntity.DateLastPurchase,
                Notes = supplierEntity.Notes
            };
        }

        public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            // se buscar la entidad en la base de datos
            var supplierEntity = await context.Suppliers
                                               .AsNoTracking() // Es un método de lectura, no necesitamos seguimiento porque si lo seguimos explota todo
                                               .FirstOrDefaultAsync(s => s.Id == id);

            if (supplierEntity == null)
            {
                return null; 
            }

            //Mapear la entidad a DTO y retornar
            return new SupplierDto
            {
                Id = id,
                Name = supplierEntity.Name,
                Address = supplierEntity.Address,
                Phone = supplierEntity.Phone,
                Email = supplierEntity.Email,
                DateLastPurchase = supplierEntity.DateLastPurchase,
                Notes = supplierEntity.Notes,
                IsActive= supplierEntity.IsActive
            };
        }

        public async Task<bool> UpdateSupplierAsync(SupplierDto updatedSupplierDto)
        {
            using var context = _contextFactory.CreateDbContext();

            var supplier = await context.Suppliers.FindAsync(updatedSupplierDto.Id);
            if (supplier == null) return false;

            bool existEmail = await context.Suppliers
                            .AnyAsync(s=> s.Id != updatedSupplierDto.Id &&  s.Email == updatedSupplierDto.Email && s.IsActive == true);
            if (existEmail)
            {
                throw new InvalidOperationException("El correo ya está registrado por un proveedor activo");
            }

            bool existPhone = await context.Suppliers
                            .AnyAsync(s => s.Id != updatedSupplierDto.Id && s.Phone == updatedSupplierDto.Phone && s.IsActive == true);
            if (existPhone)
            {
                throw new InvalidOperationException("El contacto del proveedor ya está registrado por un proveedor activo");
            }

                
            supplier.Name = updatedSupplierDto.Name;
            supplier.Address = updatedSupplierDto.Address;
            supplier.Phone = updatedSupplierDto.Phone;
            supplier.Email = updatedSupplierDto.Email;
            supplier.DateLastPurchase = updatedSupplierDto.DateLastPurchase;
            supplier.Notes = updatedSupplierDto.Notes;
            supplier.IsActive = updatedSupplierDto.IsActive;

            try
            {
                // Guardar los cambios. Retorna el número de filas afectadas (debe ser 1) para que funque
                var result = await context.SaveChangesAsync();
                return result > 0;
            }
            catch (DbUpdateConcurrencyException)
            {

                if (!context.Suppliers.Any(e => e.Id == updatedSupplierDto.Id))
                {
                    return false; // No encontrado
                }
                throw; // Lanzar otras excepciones 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar proveedor desde el metodo en el service: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var supplier = await context.Suppliers.FindAsync(id);

            if (supplier == null) return false;

            supplier.IsActive = false;

            try
            {

                var result = await context.SaveChangesAsync();
                return result > 0;
            }
            catch (DbUpdateConcurrencyException)
            {

                if (!context.Suppliers.Any(e => e.Id == id))
                {
                    return false; 
                }
                throw; // Lanzar otras excepciones 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el proveedor desde el metodo en el service: {ex.Message}");
                return false;
            }

        }

    }
}
