namespace LAFABRICA.Models.AuxiliarDTOS
{
    public class MaterialSupplierInventoryDto
    {
        public string? MaterialName { get; set; }
        public string? SupplierName { get; set; }
        public int? Quantity { get; set; } // De Material_Supplier
        public string? Unit { get; set; }
        public int? MinimumQuantity { get; set; } // De Inventory
        public decimal? PricePurchase { get; set; }
        public string? photoUrl { get; set; }
        // Puedes agregar IDs si necesitas acciones (Editar/Eliminar)
        public int MaterialId { get; set; }
        public int SupplierId { get; set; }
    }
}
