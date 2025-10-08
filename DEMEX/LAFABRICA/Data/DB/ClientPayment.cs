using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class ClientPayment
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public DateOnly PaymentDate { get; set; }

    public string? PaymentMethod { get; set; }

    public string PhotoVoucherUrl { get; set; } = null!;

    public int? OrderId { get; set; }

    public virtual Order? Order { get; set; }
}
