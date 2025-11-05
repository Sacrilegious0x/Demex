using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LAFABRICA.Data.DB;

public partial class EmployeePayment
{
    
    public int Id { get; set; }
    [Required(ErrorMessage = "Debe seleccionar un empleado.")]
    public int EmployeeId { get; set; }
    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime Date { get; set; }
    [Required(ErrorMessage = "Debe seleccionar un estado.")]
    [RegularExpression("Pendiente|Pagado|Anulado", ErrorMessage = "Estado inválido.")]
    public string? State { get; set; }

    public string? Description { get; set; }

    public decimal? TotalAmount { get; set; }
    public bool IsActive { get; set; } = true;
    // Relaciones
    public virtual User Employee { get; set; } = null!;

    public virtual ICollection<PayEmployeeProduct> PayEmployeeProducts { get; set; } = new List<PayEmployeeProduct>();
}
