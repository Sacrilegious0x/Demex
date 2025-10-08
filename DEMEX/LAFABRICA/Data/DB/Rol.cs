using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class Rol
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Administrator> Administrators { get; set; } = new List<Administrator>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
