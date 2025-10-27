using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 

namespace LAFABRICA.Data.DB;

public partial class Product
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(255)] 
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(255)] 
    public string Category { get; set; } = null!;

    [Required]
    [StringLength(255)] 
    public string Description { get; set; } = null!;

    [Required]
    public decimal PriceBase { get; set; }

    [Required]
    public byte IsCustom { get; set; }

    public string Complexity { get; set; } = null!;

    [StringLength(255)]
    public string? PhotoUrl { get; set; }

    [Required]
    public byte IsActive { get; set; }

    // --- Relaciones ---
    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();
    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
    public virtual ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
    public virtual ICollection<PayEmployeeProduct> PayEmployeeProducts { get; set; } = new List<PayEmployeeProduct>();
}