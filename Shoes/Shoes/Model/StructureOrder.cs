using System;
using System.Collections.Generic;

namespace Shoes.Model;

public partial class StructureOrder
{
    public int StructureOrderId { get; set; }

    public int? OrderId { get; set; }

    public string? TovarArticle { get; set; }

    public int? StructureOrderTovarCount { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Tovar? TovarArticleNavigation { get; set; }

    public string DisplayText
    {
        get
        {
            if (TovarArticleNavigation != null)
            {
                return $"{TovarArticleNavigation.TovarName} - {StructureOrderTovarCount} шт.";
            }
            return $"Товар {TovarArticle} - {StructureOrderTovarCount} шт.";
        }
    }
}
