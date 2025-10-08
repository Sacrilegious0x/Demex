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

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<MaterialSupplier> MaterialSuppliers { get; set; } = new List<MaterialSupplier>();

    public virtual Supplier? Supplier { get; set; }
}
