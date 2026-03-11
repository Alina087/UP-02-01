using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CosmeticApi.Model;

public partial class StructureOrder
{
    public int StructureOrderId { get; set; }

    public int? OrderId { get; set; }

    public string? TovarArticle { get; set; }

    public int? StructureOrderTovarCount { get; set; }
    [JsonIgnore]
    public virtual Order? Order { get; set; }
    [JsonIgnore]
    public virtual Tovar? TovarArticleNavigation { get; set; }
}
