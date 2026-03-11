using System;
using System.Collections.Generic;

namespace Demo.Model;

public partial class User
{
    public int UserId { get; set; }

    public int? RoleId { get; set; }

    public string? UserSurname { get; set; }

    public string? UserName { get; set; }

    public string? UserLastname { get; set; }

    public string? UserLogin { get; set; }

    public string? UserPass { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role? Role { get; set; }
    public string? RName => Role?.RoleName;
}
