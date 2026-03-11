using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CosmeticApi.Model;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string? SupplierName { get; set; }
    [JsonIgnore]
    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}
