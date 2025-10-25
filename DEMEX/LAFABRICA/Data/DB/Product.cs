using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal PriceBase { get; set; }

    public byte IsCustom { get; set; }

    public string Complexity { get; set; } = null!;

    public string? PhotoUrl { get; set; }

    public byte IsActive { get; set; }

    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    public virtual ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
}
