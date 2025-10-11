using LAFABRICA.Data.DB;
using LAFABRICA.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LAFABRICA.Services
{
    public class SupplierService : ISupplierService
    {
        // 1. REEMPLAZAMOS HttpClient por AppDbContext
        private readonly AppDbContext _context;

        // 2. CAMBIAMOS EL CONSTRUCTOR para inyectar AppDbContext
        public SupplierService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SupplierDto>> GetSuppliersAsync()
        {
            try
            {
                // 3. USAMOS EF CORE para obtener los datos
                var suppliers = await _context.Suppliers
                .Where(s => s.IsActive) // <-- ¡FILTRO AGREGADO!
                .ToListAsync();

                // 4. Mapeamos la entidad a la DTO (Si el modelo Supplier no es igual a SupplierDto)
                // Si el modelo Supplier de la BD y SupplierDto son idénticos, puedes quitar el DTO y usar la entidad,
                // pero mantener el DTO es una buena práctica para separar capas.

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
                // En un servicio in-process, es mejor lanzar la excepción o usar un logger, 
                // pero retornamos una lista vacía para la UI.
                return new List<SupplierDto>();
            }




        }

        public async Task<SupplierDto> AddSupplierAsync(SupplierDto newSupplierDto)
        {
            // 1. Mapear DTO a la Entidad de EF Core (Supplier)
            // (Asumimos que tienes una clase de entidad Supplier en tu proyecto)
            var supplierEntity = new Supplier
            {
                // El Id debe ser 0 o ignorarse, ya que es autoincremental
                Name = newSupplierDto.Name,
                Address = newSupplierDto.Address,
                Phone = newSupplierDto.Phone,
                Email = newSupplierDto.Email,
                DateLastPurchase = newSupplierDto.DateLastPurchase,
                Notes = newSupplierDto.Notes,
                IsActive = newSupplierDto.IsActive
                // Las propiedades de navegación (List<...>) se ignoran o se inicializan si es necesario.
            };

            // 2. Agregar al Contexto
            _context.Suppliers.Add(supplierEntity);

            // 3. Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            // 4. Mapear la Entidad de vuelta a DTO para retornar el objeto con el ID generado
            return new SupplierDto
            {
                Id = supplierEntity.Id, // <--- ID Autoincremental generado por la BD
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
            // 1. Buscar la entidad en la base de datos
            var supplierEntity = await _context.Suppliers
                                               .AsNoTracking() // Es un método de lectura, no necesitamos seguimiento
                                               .FirstOrDefaultAsync(s => s.Id == id);

            if (supplierEntity == null)
            {
                return null; // Retorna nulo si no se encuentra
            }

            // 2. Mapear la entidad a DTO y retornar
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

            var supplier = await _context.Suppliers.FindAsync(updatedSupplierDto.Id);
            if (supplier == null) return false;

            supplier.Name = updatedSupplierDto.Name;
            supplier.Address = updatedSupplierDto.Address;
            supplier.Phone = updatedSupplierDto.Phone;
            supplier.Email = updatedSupplierDto.Email;
            supplier.DateLastPurchase = updatedSupplierDto.DateLastPurchase;
            supplier.Notes = updatedSupplierDto.Notes;
            supplier.IsActive = updatedSupplierDto.IsActive;

            try
            {
                // 3. Guardar los cambios. Retorna el número de filas afectadas (debe ser 1)
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Manejar la excepción si el proveedor no existe (o si ha habido un conflicto)
                if (!_context.Suppliers.Any(e => e.Id == updatedSupplierDto.Id))
                {
                    return false; // No encontrado
                }
                throw; // Lanzar otras excepciones de concurrencia
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar proveedor desde el metodo en el service: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null) return false;

            supplier.IsActive = false;

            try
            {
                // 3. Guardar los cambios. Retorna el número de filas afectadas (debe ser 1)
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Manejar la excepción si el proveedor no existe (o si ha habido un conflicto)
                if (!_context.Suppliers.Any(e => e.Id == id))
                {
                    return false; // No encontrado
                }
                throw; // Lanzar otras excepciones de concurrencia
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el proveedor desde el metodo en el service: {ex.Message}");
                return false;
            }

        }

    }
}
