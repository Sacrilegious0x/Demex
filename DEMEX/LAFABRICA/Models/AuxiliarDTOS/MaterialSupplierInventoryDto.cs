using System.ComponentModel.DataAnnotations;

namespace LAFABRICA.Models.AuxiliarDTOS
{
    public class MaterialSupplierInventoryDto
    {
        // --- PROPIEDADES DEL MATERIAL ---
        [Required(ErrorMessage = "El nombre del material es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre debe tener como máximo 100 caracteres.")]
        public string? MaterialName { get; set; }

        [Required(ErrorMessage = "La unidad es obligatoria.")]
        [StringLength(20, ErrorMessage = "La unidad debe tener como máximo 20 caracteres.")]
        public string? Unit { get; set; }

        [Required(ErrorMessage = "El precio de compra es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de compra debe ser mayor a 0.")]
        public decimal? PricePurchase { get; set; }

        public string? photoUrl { get; set; }

        // --- PROPIEDADES DE INVENTARIO Y PROVEEDOR ---
        public string? SupplierName { get; set; }

        [Required(ErrorMessage = "El stock entrante es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int? Quantity { get; set; } // De Material_Supplier

        [Required(ErrorMessage = "El stock mínimo es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public int? MinimumQuantity { get; set; } // De Inventory

        public int MaterialId { get; set; } // Usado para Edición/Borrado

        [Required(ErrorMessage = "Debe seleccionar un proveedor.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un proveedor válido.")]
        public int SupplierId { get; set; }
    }
}