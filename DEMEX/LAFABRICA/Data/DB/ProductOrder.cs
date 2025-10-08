using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class ProductOrder
{
    public int IdProduct { get; set; }

    public int IdOrder { get; set; }

    public int Quantity { get; set; }

    public virtual Order IdOrderNavigation { get; set; } = null!;

    public virtual Product IdProductNavigation { get; set; } = null!;
}
