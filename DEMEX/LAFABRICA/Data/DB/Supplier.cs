using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class Supplier
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateTime? DateLastPurchase { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<MaterialSupplier> MaterialSuppliers { get; set; } = new List<MaterialSupplier>();

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
