using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LAFABRICA.Data.DB;

public partial class Product
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [StringLength(255)]
    public string Category { get; set; } = null!;

    [StringLength(255)]
    public string Description { get; set; } = null!;

    [Range(1, double.MaxValue, ErrorMessage = "El precio base debe ser mayor que cero.")]
    public decimal PriceBase { get; set; }

    public byte IsCustom { get; set; }


    [StringLength(50)]
    public string Complexity { get; set; } = null!;

    [StringLength(255)]
    public string? PhotoUrl { get; set; }

    public byte IsActive { get; set; }

    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    public virtual ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
}
