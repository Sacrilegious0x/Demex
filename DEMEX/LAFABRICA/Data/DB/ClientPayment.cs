using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Necesario para DataAnnotations
using System.ComponentModel.DataAnnotations.Schema; // Necesario para ForeignKey

namespace LAFABRICA.Data.DB;

public partial class ClientPayment
{
    public int Id { get; set; } // ID

    [Required(ErrorMessage = "El monto del pago es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
    public decimal Amount { get; set; } // MONTO

    [Required(ErrorMessage = "La fecha del pago es obligatoria.")]
    public DateOnly PaymentDate { get; set; } //FECHA

    [Required(ErrorMessage = "El método de pago es obligatorio.")]
    [StringLength(50, ErrorMessage = "El método de pago no puede exceder los 50 caracteres.")] // Opcional: Limitar longitud
    public string PaymentMethod { get; set; } = null!; // METODO (Cambiado a no nullable con = null!)

    // PhotoVoucherUrl NO es requerido
    [StringLength(255, ErrorMessage = "La URL del comprobante no puede exceder los 255 caracteres.")] // Opcional: Limitar longitud
    public string? PhotoVoucherUrl { get; set; } // COMPROBANTE (Cambiado a nullable y quitado '= null!')

    [Required(ErrorMessage = "Debe asociar el pago a una orden.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una orden válida.")]
    public int? OrderId { get; set; } // FK DE ORDENES

    [ForeignKey("OrderId")] // Especifica la clave foránea
    public virtual Order? Order { get; set; }
}
