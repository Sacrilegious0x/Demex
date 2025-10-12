using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class Material
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal? PricePurchase { get; set; }

    public string PhotoUrl { get; set; } = null!;

    public int? SupplierId { get; set; }

    // --- Propiedades de Navegación Correctas ---

    public virtual Supplier? Supplier { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<MaterialSupplier> MaterialSuppliers { get; set; } = new List<MaterialSupplier>();

    // Esta es la propiedad clave y la ÚNICA que lo relaciona con Product
    public virtual ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
}