using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class Order
{
    public int Id { get; set; }

    public DateOnly CreationDate { get; set; }

    public DateOnly DaliveryDate { get; set; }

    public string State { get; set; } = null!;

    public string Priority { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public decimal? Discount { get; set; }

    public decimal Advancement { get; set; }

    public string? ResumePath { get; set; }

    public byte IsActive { get; set; }

    public int? ClientId { get; set; }

    public int? AdminId { get; set; }

    public virtual Administrator? Admin { get; set; }

    public virtual Client? Client { get; set; }

    public virtual ICollection<ClientPayment> ClientPayments { get; set; } = new List<ClientPayment>();

    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();
}
