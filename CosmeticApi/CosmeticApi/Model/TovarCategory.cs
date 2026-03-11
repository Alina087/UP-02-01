using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CosmeticApi.Model;

public partial class TovarCategory
{
    public int TovarCategoryId { get; set; }

    public string? TovarCategoryName { get; set; }
    [JsonIgnore]
    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}
