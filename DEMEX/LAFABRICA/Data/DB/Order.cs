using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // <-- Añadido para validaciones
using System.ComponentModel.DataAnnotations.Schema; // <-- Añadido para ForeignKey

namespace LAFABRICA.Data.DB;

public partial class Order
{
    public int Id { get; set; }

    public DateOnly CreationDate { get; set; }

    public DateOnly DaliveryDate { get; set; }

    public string State { get; set; } = null!;

    public string Priority { get; set; } = null!;

    // === VALIDACIÓN AÑADIDA ===
    [Required(ErrorMessage = "El total acordado es obligatorio.")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "El total acordado debe ser mayor que cero.")] // Usamos decimal.MaxValue
    public decimal TotalAmount { get; set; }

    public decimal? Discount { get; set; } // No requerido

    public decimal Advancement { get; set; } // No requerido (puede ser 0)

    public string? ResumePath { get; set; }

    public byte IsActive { get; set; }

    // === VALIDACIÓN AÑADIDA ===
    // Si ClientId NUNCA puede ser nulo, quita el '?' de int? y ajusta el Range a 1.
    // Si PUEDE ser nulo (ej, para cotizaciones sin cliente), mantenlo como int? y quita [Required].
    [Required(ErrorMessage = "Debe seleccionar un cliente.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cliente válido.")]
    public int? ClientId { get; set; }

    public int? AdminId { get; set; }

    public virtual Administrator? Admin { get; set; }

    // === ATRIBUTO ForeignKey AÑADIDO ===
    [ForeignKey("ClientId")] // Asegura que EF sepa que 'Client' se relaciona con 'ClientId'
    public virtual Client? Client { get; set; }

    public virtual ICollection<ClientPayment> ClientPayments { get; set; } = new List<ClientPayment>();

    // Esta colección la usaremos en la lógica del .razor para validar que no esté vacía
    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();
}

