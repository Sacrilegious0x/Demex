using System;

namespace LAFABRICA.Data.DB;

public partial class PayEmployeeProduct
{
    public int Id { get; set; }

    public int EmployeePaymentId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    // Subtotal se calcula en la BD
    public decimal Subtotal { get; set; }

    // Relaciones
    public virtual EmployeePayment EmployeePayment { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
