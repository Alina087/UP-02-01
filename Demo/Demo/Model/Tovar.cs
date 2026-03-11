using System;
using System.Collections.Generic;

namespace Demo.Model;

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

    public byte[]? TovarImage { get; set; }

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual ICollection<StructureOrder> StructureOrders { get; set; } = new List<StructureOrder>();

    public virtual Supplier? Supplier { get; set; }

    public virtual TovarCategory? TovarCategory { get; set; }

    public object Photo => TovarImage != null ? TovarImage : "/Recourses/picture.png";
    public string CategoryName => $"{TovarCategory.TovarCategoryName} | {TovarName}";
    public string Color
    {
        get
        {
            if (TovarDiscount > 15)
                return "#2E8B57";
            if (TovarCount == 0)
                return "Blue";
            return "Transparent";
        }
    }

    public string? TotalCost
    {
        get
        {
            if (TovarDiscount > 0)
                return $"{TovarCost}→{TovarCost * (100 - TovarDiscount) / 100}";
            return $"{TovarCost}";
        }
    }

    public string Cost => TovarDiscount > 0 ? "Red" : "Black";


}
