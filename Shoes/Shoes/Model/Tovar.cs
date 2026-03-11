using System;
using System.Collections.Generic;
using System.Resources;
using System.Windows.Media.Imaging;
using System.IO;

namespace Shoes.Model;

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

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual ICollection<StructureOrder> StructureOrders { get; set; } = new List<StructureOrder>();

    public virtual Supplier? Supplier { get; set; }

    public virtual TovarCategory? TovarCategory { get; set; }

    //public string DisplayImage => (TovarImage == null || TovarImage == "-") ? "/Resources/picture.png" : $"/Resources/{TovarImage}";

    public string Title => $"{TovarCategory.TovarCategoryName} | {TovarName}";

    public string BackgroundColor
    {
        get
        {
            if (TovarCount == null || TovarCount == 0)
                return "#87CEEB";
            if (TovarDiscount > 15)
                return "#2E8B57";
            return "Transparent";
        }
    }

    public decimal? FinalPrice
    {
        get
        {
            if (TovarCost == null || TovarDiscount == null || TovarDiscount == 0)
                return null;

            return TovarCost * (100 - TovarDiscount) / 100;
        }
    }

    public string PriceDisplay
    {
        get
        {
            if (TovarCost == null)
                return "Цена: не указана";

            if (TovarDiscount == null || TovarDiscount == 0)
                return $"Цена: {TovarCost.Value:F2}₽";

            return $"Цена со скидкой: {FinalPrice:F2}₽";

        }
    }

    public string PriceColor => (TovarDiscount > 0) ? "Red" : "Black";

    public bool HasDiscount => TovarDiscount > 0;

    public bool InStock => TovarCount > 0;

    public string DisplayImage
    {
        get
        {
            try
            {
                string resourcesPath = @"D:\проекты сишарп\Shoes\Shoes\Resources\";

                if (string.IsNullOrEmpty(TovarImage) || TovarImage == "-")
                {
                    return resourcesPath + "picture.png";
                }

                string imagePath = resourcesPath + TovarImage;

                if (File.Exists(imagePath))
                {
                    return imagePath;
                }
                else
                {
                    return resourcesPath + "picture.png";
                }
            }
            catch
            {
                return @"D:\проекты сишарп\Shoes\Shoes\Resources\picture.png";
            }
        }
    }
}
