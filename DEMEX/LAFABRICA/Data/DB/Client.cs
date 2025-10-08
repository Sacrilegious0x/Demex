using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class Client
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Manager { get; set; } = null!;

    public string ManagerPhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string SpecificLocation { get; set; } = null!;

    public int? QuantityPurchase { get; set; }

    public byte IsFrequent { get; set; }

    public byte IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
