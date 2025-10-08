using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class RolePermission
{
    public int RoleId { get; set; }

    public string Module { get; set; } = null!;

    public byte Canview { get; set; }

    public byte Cancreate { get; set; }

    public byte Candelete { get; set; }

    public byte Canedit { get; set; }

    public virtual Rol Role { get; set; } = null!;
}
