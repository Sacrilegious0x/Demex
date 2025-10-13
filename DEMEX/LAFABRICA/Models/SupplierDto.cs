using System.ComponentModel.DataAnnotations;

namespace LAFABRICA.Models
{
    public class SupplierDto
    {


        
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La dirección es requerida.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "El número de contacto es requerido")]
        [RegularExpression(@"^\d{4}-\d+$", ErrorMessage = "El formato telefonico no es correcto")]
        public string Phone { get; set; }

        [Required(ErrorMessage="Debe ingresar un correo")]
        [EmailAddress(ErrorMessage = "Debe ingrear un correo válido.")]
        public string Email { get; set; }

        public DateTime? DateLastPurchase { get; set; }
        public string Notes { get; set; }

        public bool IsActive { get; set; }

    }
}
