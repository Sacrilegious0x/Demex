using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LAFABRICA.Data.DB;

public partial class ProductOrder
{
    public int IdProduct { get; set; }

    public int IdOrder { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
    public int Quantity { get; set; }

    public virtual Order IdOrderNavigation { get; set; } = null!;

    public virtual Product IdProductNavigation { get; set; } = null!;
}
