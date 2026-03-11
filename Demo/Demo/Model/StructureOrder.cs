using System;
using System.Collections.Generic;

namespace Demo.Model;

public partial class StructureOrder
{
    public int StructureOrderId { get; set; }

    public int? OrderId { get; set; }

    public string? TovarArticle { get; set; }

    public int? StructureOrderTovarCount { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Tovar? TovarArticleNavigation { get; set; }
}
