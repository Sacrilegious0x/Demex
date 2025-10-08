using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class Administrator
{
    public int Id { get; set; }

    public string Identification { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte? Isactive { get; set; }

    public int? RolId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Rol? Rol { get; set; }
}
