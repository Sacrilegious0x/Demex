using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LAFABRICA.Data.DB;

public partial class Client
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre es requerido.")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "El contacto es requerido.")]
    [RegularExpression(@"^\d{4}-\d+$", ErrorMessage = "El formato no es el correcto")]
    public string PhoneNumber { get; set; } = null!;
    [Required(ErrorMessage = "El nombre del encargado es requerido.")]
    public string Manager { get; set; } = null!;
    [Required(ErrorMessage = "El contacto del encargado es requerido.")]
    [RegularExpression(@"^\d{4}-\d+$", ErrorMessage = "El formato no es el correcto")]
    public string ManagerPhoneNumber { get; set; } = null!;
    [Required(ErrorMessage = "El correo es requerido.")]
    [EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = "La Ubicacion es requerida.")]
    public string Location { get; set; } = null!;
    [Required(ErrorMessage = "Debe especificar una ubicacion.")]
    public string SpecificLocation { get; set; } = null!;

    public int? QuantityPurchase { get; set; }

    public byte IsFrequent { get; set; }

    public byte IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
