using System.ComponentModel.DataAnnotations;

namespace SchoolAppointmentApp.Entities;

public record class Order
{
    public int OrderId { get; set; }
    public string CustomerId { get; set; } = null!;
    public Teacher Teacher { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public int StatusId { get; set; }
    public OrderStatus OrderStatus { get; set; } = null!;
    public int TotalCost { get; set; }
}

public record class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    [Range(1, int.MaxValue)] public int Quantity { get; set; }
    // Each individual ordered item has its own status
    public int StatusId { get; set; }
    public OrderItemStatus OrderItemStatus { get; set; } = null!;
}


