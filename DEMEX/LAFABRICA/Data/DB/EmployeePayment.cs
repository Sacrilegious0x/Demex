using System;
using System.Collections.Generic;

namespace LAFABRICA.Data.DB;

public partial class EmployeePayment
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public DateTime Date { get; set; }

    public string? State { get; set; }

    public string? Description { get; set; }

    public decimal? TotalAmount { get; set; }

    // Relaciones
    public virtual User Employee { get; set; } = null!;

    public virtual ICollection<PayEmployeeProduct> PayEmployeeProducts { get; set; } = new List<PayEmployeeProduct>();
}
