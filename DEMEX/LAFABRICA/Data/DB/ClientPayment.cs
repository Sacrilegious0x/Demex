using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Necesario para DataAnnotations
using System.ComponentModel.DataAnnotations.Schema; // Necesario para ForeignKey

namespace LAFABRICA.Data.DB;

public partial class ClientPayment
{
    public int Id { get; set; } 

    [Required(ErrorMessage = "El monto del pago es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
    public decimal Amount { get; set; } 

    [Required(ErrorMessage = "La fecha del pago es obligatoria.")]
    public DateOnly PaymentDate { get; set; } 

    [Required(ErrorMessage = "El método de pago es obligatorio.")]
    [StringLength(50, ErrorMessage = "El método de pago no puede exceder los 50 caracteres.")] 
    public string PaymentMethod { get; set; } = null!; 

    
    [StringLength(255, ErrorMessage = "La URL del comprobante no puede exceder los 255 caracteres.")] 
    public string? PhotoVoucherUrl { get; set; } 

    [Required(ErrorMessage = "Debe asociar el pago a una orden.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una orden válida.")]
    public int? OrderId { get; set; } 

    [ForeignKey("OrderId")] 
    public virtual Order? Order { get; set; }
}
