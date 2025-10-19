// LAFABRICA/Data/DB/ProductMaterial.cs
using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class ProductMaterial   // aqui definimos la relacion de muchos a muchos entre productos y materiales
{
    public int ProductId { get; set; }

    public int MaterialId { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

// Nota: "virtual", es necesario para hacer lazy Loading