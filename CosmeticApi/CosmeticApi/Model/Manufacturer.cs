using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CosmeticApi.Model;

public partial class Manufacturer
{
    public int ManufacturerId { get; set; }

    public string? ManufacturerName { get; set; }
    [JsonIgnore]
    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}
