using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CosmeticApi.Model;

public partial class Tovar
{
    public string TovarArticle { get; set; } = null!;

    public string? TovarName { get; set; }

    public string? TovarUnit { get; set; }

    public decimal? TovarCost { get; set; }

    public int? SupplierId { get; set; }

    public int? ManufacturerId { get; set; }

    public int? TovarCategoryId { get; set; }

    public int? TovarDiscount { get; set; }

    public int? TovarCount { get; set; }

    public string? TovarDescription { get; set; }

    public string? TovarImage { get; set; }
    [JsonIgnore]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    [JsonIgnore]
    public virtual Manufacturer? Manufacturer { get; set; }
    [JsonIgnore]
    public virtual ICollection<StructureOrder> StructureOrders { get; set; } = new List<StructureOrder>();
    [JsonIgnore]
    public virtual Supplier? Supplier { get; set; }
    [JsonIgnore]
    public virtual TovarCategory? TovarCategory { get; set; }

    public string? SupplierName => Supplier?.SupplierName;

    public string? ManufacturerName => Manufacturer?.ManufacturerName;

    public string? TovarCategoryName => TovarCategory?.TovarCategoryName;
}
