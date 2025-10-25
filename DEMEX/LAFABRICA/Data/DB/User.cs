using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LAFABRICA.Data.DB;

public partial class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La identificación es requerida.")]
    [RegularExpression(@"^(?!\s)(?!.*\s$).+$", ErrorMessage = "La identificación no debe tener espacios al inicio ni al final.")]
    public string Identification { get; set; } = null!;
    [Required(ErrorMessage = "El nombre es requerido.")]
    [RegularExpression(@"^(?!\s)(?!.*\s$).+$", ErrorMessage = "El nombre no debe tener espacios al inicio ni al final.")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "El correo es requerido.")]
    [EmailAddress(ErrorMessage = "Debe ingresar un correo válido.")]
    [RegularExpression(@"^\S+$", ErrorMessage = "El correo no puede contener espacios.")]
    public string Email { get; set; } = null!;
    
    public string Password { get; set; } = null!;
    [Required(ErrorMessage = "La especialidad es requerida.")]
    [RegularExpression(@"^(?!\s)(?!.*\s$).+$", ErrorMessage = "La especialidad no debe tener espacios al inicio ni al final.")]
    public string Speciality { get; set; } = null!;

    public byte? IsActive { get; set; }
    [Required(ErrorMessage = "Debe seleccionar un rol.")]
    public int? RolId { get; set; }

    public string UserType { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Rol? Rol { get; set; }
}
