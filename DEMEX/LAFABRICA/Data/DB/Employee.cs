using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class Employee
{
    public int Id { get; set; }

    public string Identification { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Speciality { get; set; } = null!;

    public byte? IsActive { get; set; }

    public int? RolId { get; set; }

    public virtual Rol? Rol { get; set; }
}
