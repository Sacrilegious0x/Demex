using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class RolePermission
{
    public int RoleId { get; set; }

    public string Module { get; set; } = null!;

    public bool Canview { get; set; }

    public bool Cancreate { get; set; }

    public bool Candelete { get; set; }

    public bool Canedit { get; set; }

    public virtual Rol Role { get; set; } = null!;
}
