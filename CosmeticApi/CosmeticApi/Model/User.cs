using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CosmeticApi.Model;

public partial class User
{
    public int UserId { get; set; }

    public int? RoleId { get; set; }

    public string? UserSurname { get; set; }

    public string? UserName { get; set; }

    public string? UserLastname { get; set; }

    public string? UserLogin { get; set; }

    public string? UserPass { get; set; }
    [JsonIgnore]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    [JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    [JsonIgnore]
    public virtual Role? Role { get; set; }

    public string? UserRole => Role?.RoleName;
}
