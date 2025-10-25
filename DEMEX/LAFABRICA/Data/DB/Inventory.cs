using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class Inventory
{
    public int Id { get; set; }

    public int? MinimunQuantity { get; set; }

    public int Quantity { get; set; }

    public string State { get; set; } = null!;

    public int? MaterialId { get; set; }

    public virtual Material? Material { get; set; }

}
