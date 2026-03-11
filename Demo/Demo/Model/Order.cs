using System;
using System.Collections.Generic;

namespace Demo.Model;

public partial class Order
{
    public int OrderId { get; set; }

    public DateOnly? OrderDate { get; set; }

    public DateOnly? OrderDateDelivery { get; set; }

    public int? PickUpPointId { get; set; }

    public int? UserId { get; set; }

    public string? OrderCode { get; set; }

    public string? OrderStatus { get; set; }

    public virtual PickUpPoint? PickUpPoint { get; set; }

    public virtual ICollection<StructureOrder> StructureOrders { get; set; } = new List<StructureOrder>();

    public virtual User? User { get; set; }
}
