using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class MaterialSupplier
{
    public int MaterialId { get; set; }

    public int SupplierId { get; set; }

    public int? Quantity { get; set; }

    public bool? IsActive { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;

    public bool? IsActive { get; set; }
}


