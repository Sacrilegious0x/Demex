// LAFABRICA/Data/DB/ProductMaterial.cs
using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class ProductMaterial
{
    public int ProductId { get; set; }

    public int MaterialId { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}