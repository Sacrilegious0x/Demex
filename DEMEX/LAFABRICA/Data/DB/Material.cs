using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LAFABRICA.Data.DB;

public partial class Material
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre del material es obligatorio.")]
    public string Name { get; set; } = null!;
    [Range(0, double.MaxValue, ErrorMessage = "El precio de compra no puede ser negativo.")]
    public decimal? PricePurchase { get; set; }

    public string PhotoUrl { get; set; } = null!;
    [Required(ErrorMessage = "La unidad es obligatoria.")]
    [StringLength(20, ErrorMessage = "La unidad debe tener como máximo 20 caracteres.")]
    public string Unit { get; set; } = null!;
    public bool? IsActive { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<MaterialSupplier> MaterialSuppliers { get; set; } = new List<MaterialSupplier>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
}
