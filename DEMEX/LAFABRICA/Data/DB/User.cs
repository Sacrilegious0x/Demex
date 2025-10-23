using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class User
{
    public int Id { get; set; }

    public string Identification { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Speciality { get; set; } = null!;

    public byte? IsActive { get; set; }

    public int? RolId { get; set; }

    public string UserType { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Rol? Rol { get; set; }
}
