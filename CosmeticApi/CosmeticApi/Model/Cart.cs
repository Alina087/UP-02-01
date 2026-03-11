using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CosmeticApi.Model;

public partial class Cart
{
    public int UserId { get; set; }

    public string TovarArticle { get; set; } = null!;

    public int? CartTovarCount { get; set; }
    [JsonIgnore]
    public virtual Tovar TovarArticleNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual User User { get; set; } = null!;

    
}
